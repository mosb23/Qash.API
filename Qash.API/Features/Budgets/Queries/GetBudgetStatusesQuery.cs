using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Budgets.DTOs;

namespace Qash.API.Features.Budgets.Queries;

public class GetBudgetStatusesQuery : IRequest<ApiResponse<List<BudgetStatusDto>>>
{
    public GetBudgetStatusesQuery(Guid userId, int year, int month)
    {
        UserId = userId;
        Year = year;
        Month = month;
    }

    public Guid UserId { get; }

    public int Year { get; }

    public int Month { get; }
}
