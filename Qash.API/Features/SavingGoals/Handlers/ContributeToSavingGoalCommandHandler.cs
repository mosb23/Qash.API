using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.Commands;
using Qash.API.Features.SavingGoals.DTOs;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class ContributeToSavingGoalCommandHandler : IRequestHandler<ContributeToSavingGoalCommand, ApiResponse<SavingGoalDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ContributeToSavingGoalCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SavingGoalDto>> Handle(ContributeToSavingGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _context.SavingGoals
            .FirstOrDefaultAsync(
                x => x.Id == request.SavingGoalId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (goal is null)
        {
            return ApiResponse<SavingGoalDto>.FailResponse(
                "Contribution failed.",
                ["Saving goal was not found."]);
        }

        goal.CurrentAmount += request.Amount;
        goal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<SavingGoalDto>.SuccessResponse(
            _mapper.Map<SavingGoalDto>(goal),
            "Contribution recorded successfully.");
    }
}
