using Qash.API.Domain.Common;
using System;

namespace Qash.API.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid WalletId { get; set; }

    public Wallet Wallet { get; set; } = null!;

    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string TransactionType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}