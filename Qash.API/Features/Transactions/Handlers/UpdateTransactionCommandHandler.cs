using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Transactions.Commands;
using Qash.API.Features.Transactions.DTOs;
using Qash.API.Infrastructure.Data;

using WalletEntity = Qash.API.Domain.Entities.Wallet;

namespace Qash.API.Features.Transactions.Handlers;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, ApiResponse<TransactionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateTransactionCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionDto>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Id == request.TransactionId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (transaction is null)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Update transaction failed.",
                ["Transaction was not found."]);
        }

        var targetWallet = await _context.Wallets
            .FirstOrDefaultAsync(
                x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (targetWallet is null)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Update transaction failed.",
                ["Target wallet was not found."]);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Update transaction failed.",
                ["Category was not found."]);
        }

        if (category.Type != request.TransactionType)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Update transaction failed.",
                ["Category type does not match transaction type."]);
        }

        ReverseEffect(transaction.Wallet, transaction.TransactionType, transaction.Amount);
        ApplyEffect(targetWallet, request.TransactionType, request.Amount);

        transaction.WalletId = targetWallet.Id;
        transaction.CategoryId = category.Id;
        transaction.Amount = request.Amount;
        transaction.TransactionType = request.TransactionType;
        transaction.Description = request.Description.Trim();
        transaction.TransactionDate = request.TransactionDate == default
            ? transaction.TransactionDate
            : request.TransactionDate;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(transaction).Reference(x => x.Wallet).LoadAsync(cancellationToken);
        await _context.Entry(transaction).Reference(x => x.Category).LoadAsync(cancellationToken);

        var dto = _mapper.Map<TransactionDto>(transaction);

        return ApiResponse<TransactionDto>.SuccessResponse(
            dto,
            "Transaction updated successfully.");
    }

    private static void ApplyEffect(WalletEntity wallet, CategoryType transactionType, decimal amount)
    {
        if (transactionType == CategoryType.Income)
        {
            wallet.Balance += amount;
            return;
        }

        wallet.Balance -= amount;
    }

    private static void ReverseEffect(WalletEntity wallet, CategoryType transactionType, decimal amount)
    {
        if (transactionType == CategoryType.Income)
        {
            wallet.Balance -= amount;
            return;
        }

        wallet.Balance += amount;
    }
}