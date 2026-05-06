using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;

namespace Qash.API.Features.SavingGoals.Queries;

public class GetSavingGoalsQuery : IRequest<ApiResponse<List<SavingGoalDto>>>
{
    public GetSavingGoalsQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
