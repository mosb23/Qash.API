using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.RecurringTransactions.Commands;
using Qash.API.Features.RecurringTransactions.DTOs;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class UpdateRecurringTransactionCommandHandler : IRequestHandler<UpdateRecurringTransactionCommand, ApiResponse<RecurringTransactionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateRecurringTransactionCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RecurringTransactionDto>> Handle(UpdateRecurringTransactionCommand request, CancellationToken cancellationToken)
    {
        var recurring = await _context.RecurringTransactions
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Id == request.RecurringTransactionId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (recurring is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Update recurring transaction failed.",
                ["Recurring transaction was not found."]);
        }

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(
                x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Update recurring transaction failed.",
                ["Wallet was not found."]);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Update recurring transaction failed.",
                ["Category was not found."]);
        }

        if (category.Type != request.TransactionType)
        {
            return ApiResponse<RecurringTransactionDto>.FailResponse(
                "Update recurring transaction failed.",
                ["Category type does not match transaction type."]);
        }

        recurring.WalletId = wallet.Id;
        recurring.CategoryId = category.Id;
        recurring.Amount = request.Amount;
        recurring.TransactionType = request.TransactionType;
        recurring.Description = string.IsNullOrWhiteSpace(request.Description)
            ? recurring.Description
            : request.Description.Trim();
        recurring.Frequency = request.Frequency;
        recurring.NextRunAt = ToUtc(request.NextRunAt);
        recurring.IsActive = request.IsActive;
        recurring.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(recurring).Reference(x => x.Wallet).LoadAsync(cancellationToken);
        await _context.Entry(recurring).Reference(x => x.Category).LoadAsync(cancellationToken);

        return ApiResponse<RecurringTransactionDto>.SuccessResponse(
            _mapper.Map<RecurringTransactionDto>(recurring),
            "Recurring transaction updated successfully.");
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
