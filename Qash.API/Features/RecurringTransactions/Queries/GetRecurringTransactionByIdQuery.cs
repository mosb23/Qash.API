using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.RecurringTransactions.DTOs;

namespace Qash.API.Features.RecurringTransactions.Queries;

public class GetRecurringTransactionByIdQuery : IRequest<ApiResponse<RecurringTransactionDto>>
{
    public GetRecurringTransactionByIdQuery(Guid userId, Guid recurringTransactionId)
    {
        UserId = userId;
        RecurringTransactionId = recurringTransactionId;
    }

    public Guid UserId { get; }

    public Guid RecurringTransactionId { get; }
}
