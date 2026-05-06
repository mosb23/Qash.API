using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.RecurringTransactions.Commands;

public class DeleteRecurringTransactionCommand : IRequest<ApiResponse<string>>
{
    public Guid UserId { get; set; }

    public Guid RecurringTransactionId { get; set; }
}
