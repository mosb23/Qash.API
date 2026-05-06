using Qash.API.Domain.Enums;

namespace Qash.API.Features.RecurringTransactions.DTOs;

public class RecurringTransactionDto
{
    public Guid RecurringTransactionId { get; set; }

    public Guid WalletId { get; set; }

    public string WalletName { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public CategoryType TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;

    public RecurringFrequency Frequency { get; set; }

    public DateTime NextRunAt { get; set; }

    public bool IsActive { get; set; }
}
