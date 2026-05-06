using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Insights.DTOs;
using Qash.API.Features.Insights.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Insights.Handlers;

public class GetInsightsQueryHandler : IRequestHandler<GetInsightsQuery, ApiResponse<InsightsResponseDto>>
{
    private const int LookbackDays = 30;

    private readonly ApplicationDbContext _context;

    public GetInsightsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<InsightsResponseDto>> Handle(GetInsightsQuery request, CancellationToken cancellationToken)
    {
        var insights = new List<InsightDto>();
        var from = DateTime.UtcNow.AddDays(-LookbackDays);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId && x.TransactionDate >= from)
            .ToListAsync(cancellationToken);

        var expenses = transactions.Where(x => x.TransactionType == CategoryType.Expense).ToList();
        var income = transactions.Where(x => x.TransactionType == CategoryType.Income).Sum(x => x.Amount);
        var expenseTotal = expenses.Sum(x => x.Amount);

        if (expenses.Count > 0 && expenseTotal > 0)
        {
            var top = expenses
                .GroupBy(x => x.CategoryId)
                .Select(g => new { g.First().Category.Name, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .First();

            var share = top.Total / expenseTotal;
            if (share >= 0.4m)
            {
                insights.Add(new InsightDto
                {
                    Message =
                        $"About {(share * 100):0}% of your spending in the last {LookbackDays} days went to \"{top.Name}\". Consider reviewing that category.",
                    Severity = "warning"
                });
            }
        }

        if (income > 0 && expenseTotal > income)
        {
            insights.Add(new InsightDto
            {
                Message =
                    $"In the last {LookbackDays} days your expenses ({expenseTotal:0.##}) exceeded your recorded income ({income:0.##}).",
                Severity = "warning"
            });
        }

        if (expenseTotal == 0 && income > 0)
        {
            insights.Add(new InsightDto
            {
                Message = "No expenses recorded in the last month — nice discipline, or you may want to log spending for better visibility.",
                Severity = "info"
            });
        }

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonth = monthStart.AddMonths(1);

        var budgets = await _context.Budgets
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId && x.Year == monthStart.Year && x.Month == monthStart.Month)
            .ToListAsync(cancellationToken);

        foreach (var budget in budgets)
        {
            var spent = await _context.Transactions
                .AsNoTracking()
                .Where(x =>
                    x.ApplicationUserId == request.UserId &&
                    x.TransactionType == CategoryType.Expense &&
                    x.CategoryId == budget.CategoryId &&
                    x.TransactionDate >= monthStart &&
                    x.TransactionDate < nextMonth)
                .SumAsync(x => x.Amount, cancellationToken);

            if (spent > budget.Amount)
            {
                insights.Add(new InsightDto
                {
                    Message =
                        $"You are over budget on \"{budget.Category.Name}\" this month (spent {spent:0.##} vs budget {budget.Amount:0.##}).",
                    Severity = "warning"
                });
            }
        }

        var goals = await _context.SavingGoals
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .ToListAsync(cancellationToken);

        foreach (var goal in goals)
        {
            if (goal.Deadline < now && goal.CurrentAmount < goal.TargetAmount)
            {
                insights.Add(new InsightDto
                {
                    Message =
                        $"Saving goal \"{goal.Name}\" passed its deadline with {goal.CurrentAmount:0.##} of {goal.TargetAmount:0.##} saved.",
                    Severity = "warning"
                });
            }
            else if ((goal.Deadline - now).TotalDays is > 0 and <= 14 && goal.CurrentAmount < goal.TargetAmount)
            {
                insights.Add(new InsightDto
                {
                    Message =
                        $"Saving goal \"{goal.Name}\" is due soon ({goal.Deadline:yyyy-MM-dd}) and is still short of its target.",
                    Severity = "info"
                });
            }
        }

        if (insights.Count == 0)
        {
            insights.Add(new InsightDto
            {
                Message = "Your finances look steady based on recent activity. Keep logging transactions for richer insights.",
                Severity = "info"
            });
        }

        return ApiResponse<InsightsResponseDto>.SuccessResponse(
            new InsightsResponseDto { Insights = insights },
            "Insights generated successfully.");
    }
}
