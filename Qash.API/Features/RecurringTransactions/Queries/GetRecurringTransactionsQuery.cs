using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.RecurringTransactions.DTOs;

namespace Qash.API.Features.RecurringTransactions.Queries;

public class GetRecurringTransactionsQuery : IRequest<ApiResponse<List<RecurringTransactionDto>>>
{
    public GetRecurringTransactionsQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
