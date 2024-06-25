using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;

namespace OutOfOffice.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmployeesController : Controller
    {
        private readonly EmployeesRepository _employeesRepository;
        private readonly IMapper _mapper;

        public EmployeesController(EmployeesRepository employeesRepository, IMapper mapper)
        {
            _employeesRepository = employeesRepository;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Create()
        {
            var allHRs = (List<Employee>)await _employeesRepository.GetAllHRsAsync();
            EmployeeBinding model = new(allHRs);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeBinding model)
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
                newEmployee.PasswordHash = PasswordHasher.HashPassword(model.Password);
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
    }
}
