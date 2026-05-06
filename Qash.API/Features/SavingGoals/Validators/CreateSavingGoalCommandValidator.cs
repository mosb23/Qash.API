using FluentValidation;
using Qash.API.Features.SavingGoals.Commands;

namespace Qash.API.Features.SavingGoals.Validators;

public class CreateSavingGoalCommandValidator : AbstractValidator<CreateSavingGoalCommand>
{
    public CreateSavingGoalCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.TargetAmount)
            .GreaterThan(0);
    }
}
