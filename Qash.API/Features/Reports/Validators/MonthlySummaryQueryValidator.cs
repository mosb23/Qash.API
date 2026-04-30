using FluentValidation;
using Qash.API.Features.Reports.Queries;

namespace Qash.API.Features.Reports.Validators;

public class MonthlySummaryQueryValidator : AbstractValidator<MonthlySummaryQuery>
{
    public MonthlySummaryQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}