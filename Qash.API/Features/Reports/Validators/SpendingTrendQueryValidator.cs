using FluentValidation;
using Qash.API.Features.Reports.Queries;

namespace Qash.API.Features.Reports.Validators;

public class SpendingTrendQueryValidator : AbstractValidator<SpendingTrendQuery>
{
    public SpendingTrendQueryValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(1, 365);
    }
}