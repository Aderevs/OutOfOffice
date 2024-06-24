using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class AdminBinding
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [UIHint("Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z0-9\s!@#$%^&*()-_+=~`{}[\]:;""'<>,.?/\\|]{6,}$", ErrorMessage = "password is too easy")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        [UIHint("Password")]
        public string PasswordConfirm { get; set; }
    }
}
