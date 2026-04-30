using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Dashboard.DTOs;
using Qash.API.Features.Dashboard.Queries;
using Qash.API.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Dashboard.Handlers;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
{
    private readonly ApplicationDbContext _context;

    public GetDashboardQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = monthStart.AddMonths(1);

        var totalBalance = await _context.Wallets
            .Where(x => x.ApplicationUserId == request.UserId)
            .SumAsync(x => x.Balance, cancellationToken);

        var monthlyIncome = await _context.Transactions
            .Where(x =>
                x.ApplicationUserId == request.UserId &&
                x.TransactionType == CategoryType.Income &&
                x.TransactionDate >= monthStart &&
                x.TransactionDate < nextMonthStart)
            .SumAsync(x => x.Amount, cancellationToken);

        var monthlyExpenses = await _context.Transactions
            .Where(x =>
                x.ApplicationUserId == request.UserId &&
                x.TransactionType == CategoryType.Expense &&
                x.TransactionDate >= monthStart &&
                x.TransactionDate < nextMonthStart)
            .SumAsync(x => x.Amount, cancellationToken);

        var recentTransactions = await _context.Transactions
            .AsNoTracking()
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId)
            .OrderByDescending(x => x.TransactionDate)
            .Take(5)
            .Select(x => new RecentTransactionDto
            {
                Id = x.Id,
                Title = x.Description,
                Amount = x.Amount,
                Type = x.TransactionType.ToString(),
                CategoryName = x.Category.Name,
                WalletName = x.Wallet.Name,
                TransactionDate = x.TransactionDate
            })
            .ToListAsync(cancellationToken);

        var topCategoriesRaw = await _context.Transactions
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x =>
                x.ApplicationUserId == request.UserId &&
                x.TransactionType == CategoryType.Expense &&
                x.TransactionDate >= monthStart &&
                x.TransactionDate < nextMonthStart)
            .GroupBy(x => new
            {
                x.CategoryId,
                x.Category.Name
            })
            .Select(g => new
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(5)
            .ToListAsync(cancellationToken);

        var topCategories = topCategoriesRaw
            .Select(x => new TopCategoryDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                TotalAmount = x.TotalAmount,
                Percentage = monthlyExpenses == 0
                    ? 0
                    : Math.Round((x.TotalAmount / monthlyExpenses) * 100, 2)
            })
            .ToList();

        var dashboard = new DashboardDto
        {
            TotalBalance = totalBalance,
            MonthlyIncome = monthlyIncome,
            MonthlyExpenses = monthlyExpenses,
            MonthlyNet = monthlyIncome - monthlyExpenses,
            RecentTransactions = recentTransactions,
            TopCategories = topCategories
        };

        return ApiResponse<DashboardDto>.SuccessResponse(
            dashboard,
            "Dashboard retrieved successfully.");
    }
}