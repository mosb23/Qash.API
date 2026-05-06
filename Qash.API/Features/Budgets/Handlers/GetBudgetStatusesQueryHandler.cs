using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Budgets.DTOs;
using Qash.API.Features.Budgets.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Budgets.Handlers;

public class GetBudgetStatusesQueryHandler : IRequestHandler<GetBudgetStatusesQuery, ApiResponse<List<BudgetStatusDto>>>
{
    private readonly ApplicationDbContext _context;

    public GetBudgetStatusesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<BudgetStatusDto>>> Handle(GetBudgetStatusesQuery request, CancellationToken cancellationToken)
    {
        var monthStart = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var budgets = await _context.Budgets
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId && x.Year == request.Year && x.Month == request.Month)
            .ToListAsync(cancellationToken);

        if (budgets.Count == 0)
        {
            return ApiResponse<List<BudgetStatusDto>>.SuccessResponse(
                [],
                "No budgets for this period.");
        }

        var categoryIds = budgets.Select(x => x.CategoryId).Distinct().ToList();

        var spentByCategory = await _context.Transactions
            .AsNoTracking()
            .Where(x =>
                x.ApplicationUserId == request.UserId &&
                x.TransactionType == CategoryType.Expense &&
                categoryIds.Contains(x.CategoryId) &&
                x.TransactionDate >= monthStart &&
                x.TransactionDate < monthEnd)
            .GroupBy(x => x.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, cancellationToken);

        var result = budgets.Select(b =>
        {
            spentByCategory.TryGetValue(b.CategoryId, out var spent);
            var remaining = b.Amount - spent;
            return new BudgetStatusDto
            {
                BudgetId = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                Year = b.Year,
                Month = b.Month,
                BudgetAmount = b.Amount,
                SpentAmount = spent,
                RemainingAmount = remaining
            };
        }).ToList();

        return ApiResponse<List<BudgetStatusDto>>.SuccessResponse(
            result,
            "Budget statuses retrieved successfully.");
    }
}
