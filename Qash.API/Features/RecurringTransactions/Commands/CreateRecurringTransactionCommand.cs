using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.RecurringTransactions.DTOs;

namespace Qash.API.Features.RecurringTransactions.Commands;

public class CreateRecurringTransactionCommand : IRequest<ApiResponse<RecurringTransactionDto>>
{
    public Guid UserId { get; set; }

    public Guid WalletId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    public CategoryType TransactionType { get; set; }

    public string Description { get; set; } = string.Empty;

    public RecurringFrequency Frequency { get; set; }

    public DateTime NextRunAt { get; set; }

    public bool IsActive { get; set; } = true;
}
