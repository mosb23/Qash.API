using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;

namespace Qash.API.Features.SavingGoals.Commands;

public class ContributeToSavingGoalCommand : IRequest<ApiResponse<SavingGoalDto>>
{
    public Guid UserId { get; set; }

    public Guid SavingGoalId { get; set; }

    public decimal Amount { get; set; }
}
