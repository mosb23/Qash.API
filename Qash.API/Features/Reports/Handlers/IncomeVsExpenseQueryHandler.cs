using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Features.Reports.DTOs;
using Qash.API.Features.Reports.Queries;
using Qash.API.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qash.API.Domain.Enums;

namespace Qash.API.Features.Reports.Handlers;

public class IncomeVsExpenseQueryHandler : IRequestHandler<IncomeVsExpenseQuery, List<IncomeVsExpenseDto>>
{
    private readonly ApplicationDbContext _context;

    public IncomeVsExpenseQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IncomeVsExpenseDto>> Handle(IncomeVsExpenseQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionDate.Year == request.Year)
            .ToListAsync(cancellationToken);

        var monthlyTotals = transactions
            .GroupBy(x => x.TransactionDate.Month)
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    Income = group.Where(x => x.TransactionType == CategoryType.Income).Sum(x => x.Amount),
                    Expenses = group.Where(x => x.TransactionType == CategoryType.Expense).Sum(x => x.Amount)
                });

        var result = new List<IncomeVsExpenseDto>(12);

        for (var month = 1; month <= 12; month++)
        {
            if (monthlyTotals.TryGetValue(month, out var totals))
            {
                result.Add(new IncomeVsExpenseDto
                {
                    Month = month,
                    Income = totals.Income,
                    Expenses = totals.Expenses
                });
            }
            else
            {
                result.Add(new IncomeVsExpenseDto
                {
                    Month = month,
                    Income = 0,
                    Expenses = 0
                });
            }
        }

        return result;
    }
}