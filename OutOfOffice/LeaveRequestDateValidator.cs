using FluentValidation;
using OutOfOffice.Models;

namespace OutOfOffice
{
    public class LeaveRequestDateValidator : AbstractValidator<LeaveRequestView>
    {
        public LeaveRequestDateValidator()
        {
            RuleFor(model => model.EndDate)
                .GreaterThan(model => model.StartDate)
                .WithMessage("Leave must has at least one day");
        }
    }
}
