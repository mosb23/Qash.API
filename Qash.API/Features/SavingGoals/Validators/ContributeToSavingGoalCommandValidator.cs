using FluentValidation;
using Qash.API.Features.SavingGoals.Commands;

namespace Qash.API.Features.SavingGoals.Validators;

public class ContributeToSavingGoalCommandValidator : AbstractValidator<ContributeToSavingGoalCommand>
{
    public ContributeToSavingGoalCommandValidator()
    {
        RuleFor(x => x.SavingGoalId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);
    }
}
