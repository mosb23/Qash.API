using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.RecurringTransactions.Commands;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class DeleteRecurringTransactionCommandHandler : IRequestHandler<DeleteRecurringTransactionCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;

    public DeleteRecurringTransactionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(DeleteRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var recurring = await _context.RecurringTransactions
            .FirstOrDefaultAsync(
                x => x.Id == request.RecurringTransactionId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (recurring is null)
        {
            return ApiResponse<string>.FailResponse(
                "Delete recurring transaction failed.",
                ["Recurring transaction was not found."]);
        }

        _context.RecurringTransactions.Remove(recurring);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.SuccessResponse(
            "Recurring transaction deleted",
            "Recurring transaction deleted successfully.");
    }
}
