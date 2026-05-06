using FluentValidation;
using Qash.API.Domain.Enums;
using Qash.API.Features.RecurringTransactions.Commands;

namespace Qash.API.Features.RecurringTransactions.Validators;

public class UpdateRecurringTransactionCommandValidator : AbstractValidator<UpdateRecurringTransactionCommand>
{
    public UpdateRecurringTransactionCommandValidator()
    {
        RuleFor(x => x.RecurringTransactionId)
            .NotEmpty();

        RuleFor(x => x.WalletId)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.TransactionType)
            .Must(x => x == CategoryType.Income || x == CategoryType.Expense)
            .WithMessage("Transaction type must be Income or Expense.");

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Frequency must be defined.");
    }
}
