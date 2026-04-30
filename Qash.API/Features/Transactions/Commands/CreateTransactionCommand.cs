using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Transactions.DTOs;
using System;

namespace Qash.API.Features.Transactions.Commands;

public class CreateTransactionCommand : IRequest<ApiResponse<TransactionDto>>
{
    public Guid UserId { get; set; }

    public Guid WalletId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}