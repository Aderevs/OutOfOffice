using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOffice.Attributes;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models.Bindings
{
    public class EmployeeCreateBinding
    {
        public EmployeeCreateBinding() { }
        public EmployeeCreateBinding(List<Employee> allHRs)
        {
            Options = [];
            foreach (var hr in allHRs)
            {
                Options.Add(new(hr.FullName, hr.ID.ToString()));
            }
        }
        public int ID { get; set; }

        [Required]
        [Display(Name ="Повне ім'я")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Підрозділ")]
        public Subdivision Subdivision { get; set; }

        [Required]
        [Display(Name = "Позиція")]
        public Position Position { get; set; }

        [Required]
        [Display(Name = "Баланс на відсутність")]
        public int OutOfOfficeBalance { get; set; } = 20;

        [Display(Name = "HR")]
        [RequiredIfOptionsNotNull]
        public string? PeoplePartnerId { get; set; }
        public List<SelectListItem>? Options { get; set; }

        [BindProperty]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg" })]
        public IFormFile? Photo { get; set; }

        public bool HasPhoto { get; set; }

    }
}
