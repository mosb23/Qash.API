using System;

namespace Qash.API.Features.Transactions.DTOs;

public class TransactionDto
{
    public Guid TransactionId { get; set; }

    public Guid WalletId { get; set; }

    public string WalletName { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; }
}