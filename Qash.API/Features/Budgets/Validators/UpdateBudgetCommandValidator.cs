using FluentValidation;
using Qash.API.Features.Budgets.Commands;

namespace Qash.API.Features.Budgets.Validators;

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(x => x.BudgetId)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
