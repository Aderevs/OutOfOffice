using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize(Roles = "Administrator,HRManager,ProjectManager")]
    public class EmployeesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly EmployeesRepository _employeesRepository;
        private readonly ProjectsRepository _projectsRepository;

        public EmployeesController(
            IMapper mapper,
            EmployeesRepository employeesRepository,
            ProjectsRepository projectsRepository)
        {
            _mapper = mapper;
            _employeesRepository = employeesRepository;
            _projectsRepository = projectsRepository;
        }

        [Authorize(Roles = "ProjectManager,HRManager")]
        public async Task<IActionResult> Index()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            IEnumerable<Employee> employeesDb;
            if (User.IsInRole("HRManager"))
            {
                employeesDb = await _employeesRepository.GetAllSubordinateEmployeesByHRIdAsync(currentUserId);
            }
            else if (User.IsInRole("ProjectManager"))
            {
                employeesDb = await _employeesRepository.GetAllSubordinateEmployeesByPMIdAsync(currentUserId);
            }
            else
            {
                employeesDb = [];
            }
            var employees = _mapper.Map<List<EmployeeView>>(employeesDb);
            return View(employees);
        }
        [Authorize(Roles = "Administrator,HRManager")]
        public async Task<IActionResult> Create()
        {
            var allHRs = (List<Employee>)await _employeesRepository.GetAllHRsAsync();
            EmployeeCreateBinding model = new(allHRs);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,HRManager")]
        public async Task<IActionResult> Create(EmployeeCreateBinding model)
        {
            if (ModelState.IsValid)
            {
                var newEmployee = _mapper.Map<Employee>(model);
                if (model.Photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.Photo.CopyTo(memoryStream);
                        byte[] photoBytes = memoryStream.ToArray();
                        newEmployee.Photo = photoBytes;
                    }
                }
                newEmployee.Salt = Guid.NewGuid();
                newEmployee.PasswordHash = PasswordHasher.HashPassword(model.Password + newEmployee.Salt.ToString());
                newEmployee.IsActive = true;
                await _employeesRepository.AddAsync(newEmployee);
                return View("Success", "You successfully add new employee");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            return View(model);
        }

        [HttpPatch]
        [Authorize(Roles = "Administrator,HRManager")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int id)
        {
            var employeeOrNull = await _employeesRepository.GetByIdOrDefaultAsync(id);
            if (employeeOrNull is Employee employee)
            {
                await _employeesRepository.ChangeStatusForCertainEmployeeAsync(employee);
                return Ok(employee.IsActive);
            }
            throw new ArgumentException("No employee with such id was found");
        }

        [Authorize(Roles = "ProjectManager,HRManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var employeeDb = await _employeesRepository.GetByIdIncludeProjectsOrDefaultAsync(id);
            if (employeeDb is not null)
            {
                var employeeView = _mapper.Map<EmployeeEditBinding>(employeeDb);
                var allProjects = await _projectsRepository.GetAllAsync();
                employeeView.SetProjectOptions(allProjects);
                return View(employeeView);
            }
            else
            {
                throw new ArgumentException("No employee with such id was found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeEditBinding model)
        {
            if (ModelState.IsValid)
            {
                var employeeDb = await _employeesRepository.GetByIdIncludeProjectsOrDefaultAsync(model.ID);
                if (employeeDb is not null)
                {
                    employeeDb.FullName = model.FullName;
                    employeeDb.Subdivision = model.Subdivision;
                    employeeDb.Position = model.Position;
                    employeeDb.OutOfOfficeBalance = model.OutOfOfficeBalance;
                    if (model.ProjectsIds != null)
                    {
                        var newProjectsIds = model.ProjectsIds.Select(id => int.Parse(id)).ToList();
                        var projectIdsToRemove = employeeDb.Projects
                            .Where(project => !newProjectsIds.Contains(project.ID))
                            .Select(project => project.ID);
                        var oldProjectsIds = employeeDb.Projects.Select(project => project.ID).ToList();
                        var projectsIdsToAdd = newProjectsIds
                            .Where(id => !oldProjectsIds.Contains(id));
                        var projectsToAdd = await _projectsRepository.GetAllByRageOfIdsAsync(projectsIdsToAdd);
                        employeeDb.Projects.AddRange(projectsToAdd);
                        var projectsToRemove = await _projectsRepository.GetAllByRageOfIdsAsync(projectIdsToRemove);
                        foreach (var project in projectsToRemove)
                        {
                            employeeDb.Projects.Remove(project);
                        }
                    }
                    if (model.Photo != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Photo.CopyTo(memoryStream);
                            byte[] photoBytes = memoryStream.ToArray();
                            employeeDb.Photo = photoBytes;
                        }
                    }
                    await _employeesRepository.UpdateAsync(employeeDb);
                    return RedirectToAction("Index");
                }

                throw new ArgumentException("Field ID is immutable, no employee with such id was found");
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> GetPhotoById([FromQuery] int id)
        {
            var employee = await _employeesRepository.GetByIdOrDefaultAsync(id);
            if (employee is not null)
            {
                if (employee.Photo != null)
                {
                    return File(employee.Photo, "image/jpeg");
                }
                throw new InvalidOperationException("employee with such id doesn't have the photo");
            }
            throw new ArgumentException("No employee with such id was found");
        }
    }
}
