using FluentValidation;
using Qash.API.Features.Reports.Queries;

namespace Qash.API.Features.Reports.Validators;

public class IncomeVsExpenseQueryValidator : AbstractValidator<IncomeVsExpenseQuery>
{
    public IncomeVsExpenseQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);
    }
}