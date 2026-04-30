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

public class CategoryBreakdownQueryHandler : IRequestHandler<CategoryBreakdownQuery, List<CategoryBreakdownDto>>
{
    private readonly ApplicationDbContext _context;

    public CategoryBreakdownQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryBreakdownDto>> Handle(CategoryBreakdownQuery request, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionType == CategoryType.Expense)
            .Where(x => x.TransactionDate.Year == request.Year && x.TransactionDate.Month == request.Month)
            .GroupBy(x => x.Category.Name)
            .Select(group => new CategoryBreakdownDto
            {
                CategoryId = group.Key,
                TotalAmount = group.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(cancellationToken);
    }
}