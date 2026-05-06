using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.RecurringTransactions.DTOs;
using Qash.API.Features.RecurringTransactions.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class GetRecurringTransactionByIdQueryHandler : IRequestHandler<GetRecurringTransactionByIdQuery, ApiResponse<RecurringTransactionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRecurringTransactionByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RecurringTransactionDto>> Handle(GetRecurringTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.RecurringTransactions
            .AsNoTracking()
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Id == request.RecurringTransactionId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (item is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Recurring transaction was not found.",
                ["Recurring transaction was not found."]);
        }

        return ApiResponse<RecurringTransactionDto>.SuccessResponse(
            _mapper.Map<RecurringTransactionDto>(item),
            "Recurring transaction retrieved successfully.");
    }
}
