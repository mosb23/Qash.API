using FluentValidation;
using Qash.API.Features.Reports.Queries;

namespace Qash.API.Features.Reports.Validators;

public class CategoryBreakdownQueryValidator : AbstractValidator<CategoryBreakdownQuery>
{
    public CategoryBreakdownQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}