using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Features.Reports.DTOs;
using Qash.API.Features.Reports.Queries;
using Qash.API.Infrastructure.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qash.API.Domain.Enums;

namespace Qash.API.Features.Reports.Handlers;

public class MonthlySummaryQueryHandler : IRequestHandler<MonthlySummaryQuery, MonthlySummaryDto>
{
    private readonly ApplicationDbContext _context;

    public MonthlySummaryQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MonthlySummaryDto> Handle(MonthlySummaryQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionDate.Year == request.Year && x.TransactionDate.Month == request.Month)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions
            .Where(x => x.TransactionType == CategoryType.Income)
            .Sum(x => x.Amount);

        var totalExpenses = transactions
            .Where(x => x.TransactionType == CategoryType.Expense)
            .Sum(x => x.Amount);

        return new MonthlySummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetBalance = totalIncome - totalExpenses,
            TransactionCount = transactions.Count
        };
    }
}