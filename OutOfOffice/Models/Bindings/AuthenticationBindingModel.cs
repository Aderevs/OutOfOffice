using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models.Bindings
{
    public class AuthenticationBindingModel
    {
        [Required]
        [Display(Name = " Повне ім'я")]
        public string? FullName { get; set; }

        [Required]
        [UIHint("Password")]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}
