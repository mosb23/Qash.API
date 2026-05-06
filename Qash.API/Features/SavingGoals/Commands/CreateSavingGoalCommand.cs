using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;

namespace Qash.API.Features.SavingGoals.Commands;

public class CreateSavingGoalCommand : IRequest<ApiResponse<SavingGoalDto>>
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public DateTime Deadline { get; set; }
}
