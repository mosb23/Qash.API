using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.DTOs;
using Qash.API.Features.SavingGoals.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class GetSavingGoalsQueryHandler : IRequestHandler<GetSavingGoalsQuery, ApiResponse<List<SavingGoalDto>>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSavingGoalsQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<SavingGoalDto>>> Handle(GetSavingGoalsQuery request, CancellationToken cancellationToken)
    {
        var goals = await _context.SavingGoals
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .OrderBy(x => x.Deadline)
            .ToListAsync(cancellationToken);

        var dto = _mapper.Map<List<SavingGoalDto>>(goals);

        return ApiResponse<List<SavingGoalDto>>.SuccessResponse(
            dto,
            "Saving goals retrieved successfully.");
    }
}
