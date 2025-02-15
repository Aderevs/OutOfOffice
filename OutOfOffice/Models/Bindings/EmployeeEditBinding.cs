using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOffice.Attributes;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models.Bindings
{
    public class EmployeeEditBinding
    {
        public void SetProjectOptions(IEnumerable<Project> projects)
        {
            AllProjects = [];
            foreach (var project in projects)
            {
                AllProjects.Add(
                    new(
                        project.ProjectType.ToString() +
                        " (" + project.StartDate.ToString() + " - " + project.EndDate.ToString() + ")",
                        project.ID.ToString()));
            }
        }
        public void SetHrOptions(IEnumerable<Employee> allHRs)
        {
            HROptions = [];
            foreach (var hr in allHRs)
            {
                HROptions.Add(new(hr.FullName, hr.ID.ToString()));
            }
        }

        [Required]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Активний:")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name ="Повне ім'я")]
        public string FullName { get; set; }

        [Required]
        [Display(Name ="Підрозділ")]
        public Subdivision Subdivision { get; set; }

        [Required]
        [Display(Name ="Позиція")]
        public Position Position { get; set; }

        [Required]
        [Display(Name = "Баланс неробочих днів")]
        public int OutOfOfficeBalance { get; set; }

        [Display(Name = "HR")]
        [RequiredIfOptionsNotNull]
        public string? PeoplePartnerId { get; set; }
        public List<SelectListItem>? HROptions { get; set; }

        [BindProperty]
        [Display(Name = "Фото")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg" })]
        public IFormFile? Photo { get; set; }

        public bool HasPhoto { get; set; }

        [Display(Name = "Проєкти")]
        public List<string>? ProjectsIds { get; set; }

        public List<SelectListItem>? AllProjects { get; set; }
    }
}
