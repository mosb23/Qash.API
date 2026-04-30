using FluentValidation;
using Qash.API.Features.Wallet.Commands;

namespace Qash.API.Features.Wallet.Validators;

public class CreateWalletCommandValidator : AbstractValidator<CreateWalletCommand>
{
  public CreateWalletCommandValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(100);

    RuleFor(x => x.Currency)
        .NotEmpty()
        .MaximumLength(10);

    RuleFor(x => x.InitialBalance)
        .GreaterThanOrEqualTo(0);
  }
}