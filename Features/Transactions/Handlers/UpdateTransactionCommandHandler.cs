using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Features.Transactions.Commands;
using Qash.API.Features.Transactions.DTOs;
using Qash.API.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            .FirstOrDefaultAsync(x => x.Id == request.TransactionId && x.ApplicationUserId == request.UserId, cancellationToken);

        if (transaction is null)
        {
            return ApiResponse<TransactionDto>.FailResponse("Update transaction failed.", ["Transaction was not found."]);
        }

        var oldWallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.Id == transaction.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

        if (oldWallet is null)
        {
            return ApiResponse<TransactionDto>.FailResponse("Update transaction failed.", ["Original wallet was not found."]);
        }

        var targetWallet = transaction.WalletId == request.WalletId
            ? oldWallet
            : await _context.Wallets.FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

        if (targetWallet is null)
        {
            return ApiResponse<TransactionDto>.FailResponse("Update transaction failed.", ["Target wallet was not found."]);
        }

        ReverseEffect(oldWallet, transaction.TransactionType, transaction.Amount);
        ApplyEffect(targetWallet, request.TransactionType, request.Amount);

        transaction.WalletId = targetWallet.Id;
        transaction.Amount = request.Amount;
        transaction.TransactionType = NormalizeType(request.TransactionType);
        transaction.Category = request.Category.Trim();
        transaction.Description = request.Description.Trim();
        transaction.TransactionDate = request.TransactionDate == default ? transaction.TransactionDate : request.TransactionDate;

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(transaction).Reference(x => x.Wallet).LoadAsync(cancellationToken);
        var dto = _mapper.Map<TransactionDto>(transaction);

        return ApiResponse<TransactionDto>.SuccessResponse(dto, "Transaction updated successfully.");
    }

    private static string NormalizeType(string transactionType)
    {
        return transactionType.Trim().Equals("Income", StringComparison.OrdinalIgnoreCase)
            ? "Income"
            : "Expense";
    }

    private static void ApplyEffect(WalletEntity wallet, string transactionType, decimal amount)
    {
        if (transactionType.Trim().Equals("Income", StringComparison.OrdinalIgnoreCase))
        {
            wallet.Balance += amount;
            return;
        }

        wallet.Balance -= amount;
    }

    private static void ReverseEffect(WalletEntity wallet, string transactionType, decimal amount)
    {
        if (transactionType.Trim().Equals("Income", StringComparison.OrdinalIgnoreCase))
        {
            wallet.Balance -= amount;
            return;
        }

        wallet.Balance += amount;
    }
}