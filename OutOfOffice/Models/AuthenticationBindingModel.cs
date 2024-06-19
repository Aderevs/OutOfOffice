using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class AuthenticationBindingModel
    {
        [Required]
        public string FullName {  get; set; }

        [Required] 
        public string Password { get; set; }
    }
}
