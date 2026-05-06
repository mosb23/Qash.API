using Qash.API.Domain.Common;
using Qash.API.Domain.Enums;

namespace Qash.API.Domain.Entities;

public class RecurringTransaction : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public Guid WalletId { get; set; }

    public Wallet Wallet { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public decimal Amount { get; set; }

    public CategoryType TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;

    public RecurringFrequency Frequency { get; set; }

    public DateTime NextRunAt { get; set; }

    public bool IsActive { get; set; } = true;
}
