using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;
using Qash.API.Features.SavingGoals.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class GetSavingGoalByIdQueryHandler : IRequestHandler<GetSavingGoalByIdQuery, ApiResponse<SavingGoalDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSavingGoalByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<SavingGoalDto>> Handle(GetSavingGoalByIdQuery request, CancellationToken cancellationToken)
    {
        var goal = await _context.SavingGoals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.SavingGoalId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (goal is null)
        {
            return ApiResponse<SavingGoalDto>.FailResponse(
                "Saving goal was not found.",
                ["Saving goal was not found."]);
        }

        return ApiResponse<SavingGoalDto>.SuccessResponse(
            _mapper.Map<SavingGoalDto>(goal),
            "Saving goal retrieved successfully.");
    }
}
