using OutOfOffice.Models;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice
{
    public class RequiredIfOptionsNotNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance as EmployeeBinding;
            if (instance != null && instance.Options != null && value == null)
            {
                return new ValidationResult("The PeoplePartnerId field is required when Options is not null.");
            }
            return ValidationResult.Success;
        }
    }
}
