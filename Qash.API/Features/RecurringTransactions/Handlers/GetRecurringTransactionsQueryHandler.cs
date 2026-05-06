using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.RecurringTransactions.DTOs;
using Qash.API.Features.RecurringTransactions.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class GetRecurringTransactionsQueryHandler : IRequestHandler<GetRecurringTransactionsQuery, ApiResponse<List<RecurringTransactionDto>>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRecurringTransactionsQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RecurringTransactionDto>>> Handle(GetRecurringTransactionsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.RecurringTransactions
            .AsNoTracking()
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId)
            .OrderBy(x => x.NextRunAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<RecurringTransactionDto>>.SuccessResponse(
            _mapper.Map<List<RecurringTransactionDto>>(items),
            "Recurring transactions retrieved successfully.");
    }
}
