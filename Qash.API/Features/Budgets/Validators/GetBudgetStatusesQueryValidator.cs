using FluentValidation;
using Qash.API.Features.Budgets.Queries;

namespace Qash.API.Features.Budgets.Validators;

public class GetBudgetStatusesQueryValidator : AbstractValidator<GetBudgetStatusesQuery>
{
    public GetBudgetStatusesQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}
