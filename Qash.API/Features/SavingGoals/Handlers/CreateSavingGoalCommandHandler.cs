using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Features.SavingGoals.Commands;
using Qash.API.Features.SavingGoals.DTOs;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class CreateSavingGoalCommandHandler : IRequestHandler<CreateSavingGoalCommand, ApiResponse<SavingGoalDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateSavingGoalCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SavingGoalDto>> Handle(CreateSavingGoalCommand request, CancellationToken cancellationToken)
    {
        var deadlineUtc = ToUtc(request.Deadline);

        if (deadlineUtc <= DateTime.UtcNow)
        {
            return ApiResponse<SavingGoalDto>.FailResponse(
                "Create saving goal failed.",
                ["Deadline must be in the future."]);
        }

        var goal = new SavingGoal
        {
            ApplicationUserId = request.UserId,
            Name = request.Name.Trim(),
            TargetAmount = request.TargetAmount,
            CurrentAmount = 0,
            Deadline = deadlineUtc
        };

        await _context.SavingGoals.AddAsync(goal, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<SavingGoalDto>.SuccessResponse(
            _mapper.Map<SavingGoalDto>(goal),
            "Saving goal created successfully.");
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
