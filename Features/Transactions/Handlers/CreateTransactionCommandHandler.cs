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

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ApiResponse<TransactionDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<TransactionDto>.FailResponse("Create transaction failed.", ["Wallet was not found."]);
        }

        var normalizedType = NormalizeType(request.TransactionType);
        var transaction = new Transaction
        {
            ApplicationUserId = request.UserId,
            WalletId = wallet.Id,
            Amount = request.Amount,
            TransactionType = normalizedType,
            Category = request.Category.Trim(),
            Description = request.Description.Trim(),
            TransactionDate = request.TransactionDate == default ? DateTime.UtcNow : request.TransactionDate
        };

        ApplyEffect(wallet, transaction.TransactionType, transaction.Amount);

        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(transaction).Reference(x => x.Wallet).LoadAsync(cancellationToken);

        var dto = _mapper.Map<TransactionDto>(transaction);

        return ApiResponse<TransactionDto>.SuccessResponse(dto, "Transaction created successfully.");
    }

    private static string NormalizeType(string transactionType)
    {
        return transactionType.Trim().Equals("Income", StringComparison.OrdinalIgnoreCase)
            ? "Income"
            : "Expense";
    }

    private static void ApplyEffect(WalletEntity wallet, string transactionType, decimal amount)
    {
        if (transactionType.Equals("Income", StringComparison.OrdinalIgnoreCase))
        {
            wallet.Balance += amount;
            return;
        }

        wallet.Balance -= amount;
    }
}