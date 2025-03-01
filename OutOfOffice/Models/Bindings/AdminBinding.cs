using Microsoft.AspNetCore.Mvc;
using OutOfOffice.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models.Bindings
{
    public class AdminBinding
    {
        [Required]
        [Display(Name = "Повне ім'я")]
        public string FullName { get; set; }

        [Required]
        [UIHint("Password")]
        [Display(Name = "Пароль")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z0-9\s!@#$%^&*()-_+=~`{}[\]:;""'<>,.?/\\|]{6,}$", ErrorMessage = "пароль найдто легкий")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "паролі не співпадають")]
        [UIHint("Password")]
        [Display(Name = "Підтвердити пароль")]
        public string PasswordConfirm { get; set; }

        [BindProperty]
        [Display(Name = "Фото")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg" })]
        public IFormFile? Photo { get; set; }
    }
}
