using Microsoft.AspNetCore.Mvc;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class EmployeeEditBinding
    {
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

    }
}
