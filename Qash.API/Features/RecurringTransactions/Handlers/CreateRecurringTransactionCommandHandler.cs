using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Domain.Enums;
using Qash.API.Features.RecurringTransactions.Commands;
using Qash.API.Features.RecurringTransactions.DTOs;
using Qash.API.Infrastructure.Data;

using RecurringEntity = Qash.API.Domain.Entities.RecurringTransaction;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class CreateRecurringTransactionCommandHandler : IRequestHandler<CreateRecurringTransactionCommand, ApiResponse<RecurringTransactionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateRecurringTransactionCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RecurringTransactionDto>> Handle(CreateRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(
                x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Create recurring transaction failed.",
                ["Wallet was not found."]);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Create recurring transaction failed.",
                ["Category was not found."]);
        }

        if (category.Type != request.TransactionType)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Create recurring transaction failed.",
                ["Category type does not match transaction type."]);
        }

        var nextRunUtc = ToUtc(request.NextRunAt);

        var entity = new RecurringEntity
        {
            ApplicationUserId = request.UserId,
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Amount = request.Amount,
            TransactionType = request.TransactionType,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? "Recurring"
                : request.Description.Trim(),
            Frequency = request.Frequency,
            NextRunAt = nextRunUtc,
            IsActive = request.IsActive
        };

        await _context.RecurringTransactions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(x => x.Wallet).LoadAsync(cancellationToken);
        await _context.Entry(entity).Reference(x => x.Category).LoadAsync(cancellationToken);

        return ApiResponse<RecurringTransactionDto>.SuccessResponse(
            _mapper.Map<RecurringTransactionDto>(entity),
            "Recurring transaction created successfully.");
    }

    private static DateTime ToUtc(DateTime d)
    {
        return d.Kind switch
        {
            DateTimeKind.Utc => d,
            DateTimeKind.Local => d.ToUniversalTime(),
            _ => DateTime.SpecifyKind(d, DateTimeKind.Utc)
        };
    }
}
