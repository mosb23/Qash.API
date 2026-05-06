using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Budgets.Commands;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Budgets.Handlers;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;

    public DeleteBudgetCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(
                x => x.Id == request.BudgetId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (budget is null)
        {
            return ApiResponse<string>.FailResponse(
                "Delete budget failed.",
                ["Budget was not found."]);
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.SuccessResponse("Budget deleted", "Budget deleted successfully.");
    }
}
