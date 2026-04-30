using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Features.Reports.DTOs;
using Qash.API.Features.Reports.Queries;
using Qash.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Reports.Handlers;

public class SpendingTrendQueryHandler : IRequestHandler<SpendingTrendQuery, List<SpendingTrendDto>>
{
    private readonly ApplicationDbContext _context;

    public SpendingTrendQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SpendingTrendDto>> Handle(SpendingTrendQuery request, CancellationToken cancellationToken)
    {
        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddDays(-(request.Days - 1));

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionType == "Expense")
            .Where(x => x.TransactionDate >= startDate && x.TransactionDate < endDate.AddDays(1))
            .ToListAsync(cancellationToken);

        var totalsByDate = transactions
            .GroupBy(x => x.TransactionDate.Date)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Amount));

        var result = new List<SpendingTrendDto>(request.Days);

        for (var offset = 0; offset < request.Days; offset++)
        {
            var currentDate = startDate.AddDays(offset);
            totalsByDate.TryGetValue(currentDate, out var totalExpenses);

            result.Add(new SpendingTrendDto
            {
                Date = currentDate,
                TotalExpenses = totalExpenses
            });
        }

        return result;
    }
}