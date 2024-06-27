using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    [Authorize(Roles = "Administrator,HRManager")]
    public class EmployeesController : Controller
    {
        private readonly EmployeesRepository _employeesRepository;
        private readonly IMapper _mapper;

        public EmployeesController(EmployeesRepository employeesRepository, IMapper mapper)
        {
            _employeesRepository = employeesRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("HRManager"))
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var employeesDb = await _employeesRepository.GetAllSubordinateEmployeesByHRIdAsync(currentUserId);
                var employees = _mapper.Map<List<EmployeeView>>(employeesDb);
                return View(employees);
            }
            return View();
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

        public async Task<IActionResult> Edit(int id)
        {
            var employeeDb = await _employeesRepository.GetByIdOrDefaultAsync(id);
            if (employeeDb is Employee)
            {
                var employeeView = _mapper.Map<EmployeeEditBinding>(employeeDb);
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
                var employeeDb = await _employeesRepository.GetByIdOrDefaultAsync(model.ID);
                if (employeeDb is not null)
                {
                    employeeDb.FullName = model.FullName;
                    employeeDb.Subdivision = model.Subdivision;
                    employeeDb.Position = model.Position;
                    employeeDb.OutOfOfficeBalance = model.OutOfOfficeBalance;
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
