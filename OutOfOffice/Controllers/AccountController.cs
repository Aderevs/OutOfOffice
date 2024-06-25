using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using OutOfOffice.DbLogic.Repositories;
using OutOfOffice.Models;
using System.Security.Claims;

namespace OutOfOffice.Controllers
{
    public class AccountController : Controller
    {
        private readonly EmployeesRepository _employeesRepository;

        public AccountController(EmployeesRepository employeesRepository)
        {
            _employeesRepository = employeesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(AuthenticationBindingModel model)
        {

            if (ModelState.IsValid)
            {
                var userOrNull = await _employeesRepository.GetEmployeeByFullNameOrDefaultAsync(model.FullName);
                if (userOrNull is Employee employee)
                {
                    var isCorrectPassword = PasswordHasher.IsCorrectPassword(employee, model.Password);
                    if (isCorrectPassword)
                    {
                        await SignInAsync(employee);
                        return Ok();//RedirectToAction("Index", "Tests");
                    }
                }
                ModelState.AddModelError("", "Wrong login or password");
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> RegisterFirstAdmin()
        {
            if (await _employeesRepository.CheckIfAdminAlreadyExistsAsync())
            {
                return View("Error", "This application already have at least one administrator, so if you want to create new one, ask them to do it");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterFirstAdmin(AdminBinding model) 
        {
            if (ModelState.IsValid)
            {
                var newUser = new Employee
                {
                    FullName = model.FullName,
                    Salt = Guid.NewGuid(),
                    Subdivision = Subdivision.CompanySystemsAdministration,
                    Position = Position.Administrator,
                    IsActive = true,
                    OutOfOfficeBalance = 20,
                };
                if (model.Photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.Photo.CopyTo(memoryStream);
                        byte[] photoBytes = memoryStream.ToArray();
                        newUser.Photo = photoBytes;
                    }
                }
                newUser.PasswordHash = PasswordHasher.HashPassword(model.Password + newUser.Salt.ToString());
                await _employeesRepository.AddAsync(newUser);
                await SignInAsync(newUser);
                return View("Success", "You successfully registered first admin");
            }
            return View(model);
        }
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private async Task SignInAsync(Employee user)
        {
            string role = user.Position.ToString();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme, 
                ClaimTypes.Name, 
                ClaimTypes.Role);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
        }
    }
}
