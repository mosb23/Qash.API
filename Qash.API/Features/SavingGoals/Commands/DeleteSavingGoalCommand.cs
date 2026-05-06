using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.SavingGoals.Commands;

public class DeleteSavingGoalCommand : IRequest<ApiResponse<string>>
{
    public DeleteSavingGoalCommand(Guid userId, Guid savingGoalId)
    {
        UserId = userId;
        SavingGoalId = savingGoalId;
    }

    public Guid UserId { get; }

    public Guid SavingGoalId { get; }
}
