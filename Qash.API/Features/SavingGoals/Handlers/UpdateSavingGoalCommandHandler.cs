using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.Commands;
using Qash.API.Features.SavingGoals.DTOs;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class UpdateSavingGoalCommandHandler : IRequestHandler<UpdateSavingGoalCommand, ApiResponse<SavingGoalDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateSavingGoalCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SavingGoalDto>> Handle(UpdateSavingGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _context.SavingGoals
            .FirstOrDefaultAsync(
                x => x.Id == request.SavingGoalId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (goal is null)
        {
            return ApiResponse<SavingGoalDto>.FailResponse(
                "Update saving goal failed.",
                ["Saving goal was not found."]);
        }

        goal.Name = request.Name.Trim();
        goal.TargetAmount = request.TargetAmount;
        goal.Deadline = ToUtc(request.Deadline);
        goal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<SavingGoalDto>.SuccessResponse(
            _mapper.Map<SavingGoalDto>(goal),
            "Saving goal updated successfully.");
    }

    private static DateTime ToUtc(DateTime d)
    {
        return d.Kind switch
        {
            DateTimeKind.Utc => d,
            DateTimeKind.Local => d.ToUniversalTime(),
            _ => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        };
    }
}
