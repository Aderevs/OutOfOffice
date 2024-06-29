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
                .WithMessage("Leave must has at least one day");
            RuleFor(model => model.StartDate)
                .GreaterThanOrEqualTo(model => new DateOnly(today.Year, today.Month, today.Day))
                .WithMessage("Leave mustn't start earlier than today");
        }
    }
}
