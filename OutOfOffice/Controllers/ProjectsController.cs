using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ProjectsRepository _projectsRepository;
        private readonly DateValidator _dateValidator;

        public ProjectsController(
            IMapper mapper,
            ProjectsRepository projectsRepository,
            DateValidator dateValidator)
        {
            _mapper = mapper;
            _projectsRepository = projectsRepository;
            _dateValidator = dateValidator;
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
            return View(projectView);
        }

        [HttpPost]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> Edit(ProjectView model)
        {
            var validResult = _dateValidator.Validate(model);
            if (ModelState.IsValid && validResult.IsValid)
            {
                var projectDb = _mapper.Map<Project>(model);
                await _projectsRepository.UpdateAsync(projectDb);
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in validResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }
        }

        [Authorize(Roles = "ProjectManager")]
        public IActionResult Create()
        {
            return View("Certain");
        }
        [HttpPost]
        [Authorize(Roles = "ProjectManager")]
        public async Task<IActionResult> EditOrCreateIfNotExists(ProjectView model)
        {
            var validResult = _dateValidator.Validate(model);
            if (ModelState.IsValid && validResult.IsValid)
            {
                var projectDb = _mapper.Map<Project>(model);
                projectDb.ProjectManagerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (model.ID == null)
                {
                    projectDb.IsActive = true;
                    await _projectsRepository.AddAsync(projectDb);
                }
                else
                {
                    await _projectsRepository.UpdateAsync(projectDb);
                }
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in validResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }
        }

        [HttpPatch]
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
