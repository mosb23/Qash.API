using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.SavingGoals.Commands;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.SavingGoals.Handlers;

public class DeleteSavingGoalCommandHandler : IRequestHandler<DeleteSavingGoalCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;

    public DeleteSavingGoalCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(DeleteSavingGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _context.SavingGoals
            .FirstOrDefaultAsync(
                x => x.Id == request.SavingGoalId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (goal is null)
        {
            return ApiResponse<string>.FailResponse(
                "Delete saving goal failed.",
                ["Saving goal was not found."]);
        }

        _context.SavingGoals.Remove(goal);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.SuccessResponse("Saving goal deleted", "Saving goal deleted successfully.");
    }
}
