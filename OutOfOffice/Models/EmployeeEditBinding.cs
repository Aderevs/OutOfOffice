using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class EmployeeEditBinding
    {
        public void SetProjectOptions(IEnumerable<Project> projects)
        {
            AllProjects = [];
            foreach(var project in projects)
            {
                AllProjects.Add(
                    new(
                        project.ProjectType.ToString() + 
                        " (" + project.StartDate.ToString() + " - " + project.EndDate.ToString() + ")",
                        project.ID.ToString()));
            }
        }

        [Required]
        public int ID { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        public Subdivision Subdivision { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        public int OutOfOfficeBalance { get; set; }

        [BindProperty]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg"/*, ".png", ".gif"*/ })]
        public IFormFile? Photo { get; set; }

        public bool HasPhoto { get; set; }

        [Display(Name = "Projects")]
        public List<string>? ProjectsIds { get; set; }

        public List<SelectListItem>? AllProjects { get; set; }
    }
}
