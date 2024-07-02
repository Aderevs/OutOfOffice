using FluentValidation;
using OutOfOffice.Models;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice
{
    public class DateValidator : AbstractValidator<IDateRange>
    {
        public DateValidator()
        {
            var today = DateTime.Now;
            RuleFor(model => model.EndDate)
                .GreaterThan(model => model.StartDate)
                .WithMessage("Duration must be at least one day");
           /* RuleFor(model => model.StartDate)
                .GreaterThanOrEqualTo(model => new DateOnly(today.Year, today.Month, today.Day))
                .WithMessage("Start mustn't be earlier than today");*/
        }
    }
}
