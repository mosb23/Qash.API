using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;

namespace Qash.API.Features.SavingGoals.Queries;

public class GetSavingGoalByIdQuery : IRequest<ApiResponse<SavingGoalDto>>
{
    public GetSavingGoalByIdQuery(Guid userId, Guid savingGoalId)
    {
        UserId = userId;
        SavingGoalId = savingGoalId;
    }

    public Guid UserId { get; }

    public Guid SavingGoalId { get; }
}
