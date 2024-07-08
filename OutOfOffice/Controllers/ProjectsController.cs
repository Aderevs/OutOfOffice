//#define DEBUG
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories.Interfaces;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly DateValidator _dateValidator;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IEmployeesRepository _employeesRepository;

        public ProjectsController(
            IMapper mapper,
            DateValidator dateValidator,
            IProjectsRepository projectsRepository,
            IEmployeesRepository employeesRepository)
        {
            _mapper = mapper;
            _dateValidator = dateValidator;
            _projectsRepository = projectsRepository;
            _employeesRepository = employeesRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<ProjectView> projects;
            List<Project> projectsDb;
            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (User.IsInRole("Employee"))
            {
                projectsDb = (await _projectsRepository
                    .GetAllByEmployeeIdIncludePMAsync(id))
                    .ToList();
            }
            else if (User.IsInRole("HRManager"))
            {
                projectsDb = (await _projectsRepository
                    .GetAllProjectsOfSubordinateEmployeesByHRIdIncludePMAsync(id))
                    .ToList();
            }
            else if (User.IsInRole("ProjectManager"))
            {
                projectsDb = (await _projectsRepository
                    .GetAllByPMIdAsync(id))
                    .ToList();
            }
            else
            {
                projectsDb = new List<Project>();
            }
            projects = _mapper.Map<List<ProjectView>>(projectsDb);
            return View(projects);
        }
        public async Task<IActionResult> Certain([FromQuery] int id)
        {
            var projectDb = await _projectsRepository.GetByIdIncludeEmployeesAndPMOrDefaultAsync(id);
            if (projectDb == null)
            {
                throw new ArgumentException("No project with such id was found");
            }
            var projectView = _mapper.Map<ProjectView>(projectDb);
            var allEmployees = await _employeesRepository.GetAllEmployeesAsync();
            projectView.SetOptionsEmployees(allEmployees);
            return View(projectView);
        }

        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Create()
        {
            var allEmployees = await _employeesRepository.GetAllEmployeesAsync();
            var model = new ProjectView(allEmployees);
            return View("Certain", model);
        }
        [HttpPost]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> EditOrCreateIfNotExists(ProjectView model)
        {
            var validResult = _dateValidator.Validate(model);
            if (ModelState.IsValid && validResult.IsValid)
            {
                var pmId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<int> newInvolvedIds = null;
                if (model.EmployeesIds != null)
                {
                    newInvolvedIds = model.EmployeesIds.Select(id => int.Parse(id)).ToList();
                }
                if (model.ID == null)
                {
                    var projectDb = _mapper.Map<Project>(model);
                    projectDb.ProjectManagerId = pmId;

                    if (newInvolvedIds != null)
                        projectDb.Employees = (List<Employee>)(await _employeesRepository.GetAllByRageOfIdsAsync(newInvolvedIds));

                    projectDb.IsActive = true;
                    await _projectsRepository.AddAsync(projectDb);
                }
                else
                {
                    var projectDb = await _projectsRepository.GetByIdIncludeEmployeesOrDefaultAsync((int)model.ID);
                    if (projectDb == null)
                    {
                        throw new ArgumentException("No employee with such id was found");
                    }
                    projectDb.ProjectType = projectDb.ProjectType != model.ProjectType ? model.ProjectType : projectDb.ProjectType;
                    projectDb.StartDate = projectDb.StartDate != model.StartDate ? model.StartDate : projectDb.StartDate;
                    projectDb.EndDate = projectDb.EndDate != model.EndDate ? model.EndDate : projectDb.EndDate;
                    projectDb.Comment = projectDb.Comment != model.Comment ? model.Comment : projectDb.Comment;
                    if (newInvolvedIds != null)
                    {
                        var employeeIdsToRemove = projectDb.Employees
                            .Where(employee => !newInvolvedIds.Contains(employee.ID))
                            .Select(employee => employee.ID);
                        var oldInvolvesIds = projectDb.Employees.Select(employee => employee.ID).ToList();
                        var employeeIdsToAdd = newInvolvedIds
                            .Where(id => !oldInvolvesIds.Contains(id));
                        var employeesToAdd = await _employeesRepository.GetAllByRageOfIdsAsync(employeeIdsToAdd);
                        projectDb.Employees.AddRange(employeesToAdd);
                        var employeesToRemove = await _employeesRepository.GetAllByRageOfIdsAsync(employeeIdsToRemove);
                        foreach (var employee in employeesToRemove)
                        {
                            projectDb.Employees.Remove(employee);
                        }
                    }
                    await _projectsRepository.UpdateAsync(projectDb);
                }
                return RedirectToAction("Index");
            }
            else
            {
#if DEBUG
                var errorFields = ModelState.Where(x => x.Value.Errors.Any())
                                   .Select(x => new { x.Key, x.Value.Errors });
                foreach (var errorField in errorFields)
                {
                    Console.WriteLine(errorField);
                }
#endif
                foreach (var error in validResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                model.SetOptionsEmployees(await _employeesRepository.GetAllEmployeesAsync());
                return View("Certain", model);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int id)
        {
            var employeeOrNull = await _projectsRepository.GetByIdOrDefaultAsync(id);
            if (employeeOrNull is Project project)
            {
                await _projectsRepository.ChangeStatusForCertainProjectAsync(project);
                return Ok(project.IsActive);
            }
            throw new ArgumentException("No employee with such id was found");
        }
    }
}
