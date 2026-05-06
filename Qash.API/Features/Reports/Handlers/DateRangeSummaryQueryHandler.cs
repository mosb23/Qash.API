using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Domain.Enums;
using Qash.API.Features.Reports.DTOs;
using Qash.API.Features.Reports.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Reports.Handlers;

public class DateRangeSummaryQueryHandler : IRequestHandler<DateRangeSummaryQuery, DateRangeSummaryDto>
{
    private readonly ApplicationDbContext _context;

    public DateRangeSummaryQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DateRangeSummaryDto> Handle(DateRangeSummaryQuery request, CancellationToken cancellationToken)
    {
        var from = NormalizeStartUtc(request.FromUtc);
        var toExclusive = NormalizeEndExclusiveUtc(request.ToUtc);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionDate >= from && x.TransactionDate < toExclusive)
            .ToListAsync(cancellationToken);

        var income = transactions.Where(x => x.TransactionType == CategoryType.Income).Sum(x => x.Amount);
        var expenses = transactions.Where(x => x.TransactionType == CategoryType.Expense).Sum(x => x.Amount);

        return new DateRangeSummaryDto
        {
            FromUtc = from,
            ToUtcExclusive = toExclusive,
            TotalIncome = income,
            TotalExpenses = expenses,
            Net = income - expenses,
            TransactionCount = transactions.Count
        };
    }

    private static DateTime NormalizeStartUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    private static DateTime NormalizeEndExclusiveUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
    }
}
