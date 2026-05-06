using FluentValidation;
using Qash.API.Features.Reports.Queries;

namespace Qash.API.Features.Reports.Validators;

public class DateRangeSummaryQueryValidator : AbstractValidator<DateRangeSummaryQuery>
{
    public DateRangeSummaryQueryValidator()
    {
        RuleFor(x => x.FromUtc)
            .LessThanOrEqualTo(x => x.ToUtc)
            .WithMessage("From date must be before or equal to To date.");

        RuleFor(x => x)
            .Must(x => (x.ToUtc - x.FromUtc).TotalDays <= 732)
            .WithMessage("Date range cannot exceed two years.");
    }
}
