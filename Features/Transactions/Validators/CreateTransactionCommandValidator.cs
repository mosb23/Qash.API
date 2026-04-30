using FluentValidation;
using Qash.API.Features.Transactions.Commands;

namespace Qash.API.Features.Transactions.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.TransactionType)
            .NotEmpty()
            .Must(BeValidTransactionType)
            .WithMessage("Transaction type must be Income or Expense.");

        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }

    private static bool BeValidTransactionType(string transactionType)
    {
        return transactionType.Equals("Income", StringComparison.OrdinalIgnoreCase)
            || transactionType.Equals("Expense", StringComparison.OrdinalIgnoreCase);
    }
}