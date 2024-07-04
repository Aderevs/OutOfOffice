using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models.Bindings
{
    public class AuthenticationBindingModel
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        [UIHint("Password")]
        public string? Password { get; set; }
    }
}
