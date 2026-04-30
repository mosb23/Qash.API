using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Domain.Enums;
using Qash.API.Features.Transactions.Commands;
using Qash.API.Features.Transactions.DTOs;
using Qash.API.Infrastructure.Data;

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
            .FirstOrDefaultAsync(
                x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (wallet is null)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Create transaction failed.",
                ["Wallet was not found."]);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Create transaction failed.",
                ["Category was not found."]);
        }

        if (category.Type != request.TransactionType)
        {
            return ApiResponse<TransactionDto>.FailResponse(
                "Create transaction failed.",
                ["Category type does not match transaction type."]);
        }

        var transaction = new Transaction
        {
            ApplicationUserId = request.UserId,
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Amount = request.Amount,
            TransactionType = request.TransactionType,
            Description = request.Description.Trim(),
            TransactionDate = request.TransactionDate == default
                ? DateTime.UtcNow
                : request.TransactionDate
        };

        ApplyEffect(wallet, transaction.TransactionType, transaction.Amount);

        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(transaction).Reference(x => x.Wallet).LoadAsync(cancellationToken);
        await _context.Entry(transaction).Reference(x => x.Category).LoadAsync(cancellationToken);

        var dto = _mapper.Map<TransactionDto>(transaction);

        return ApiResponse<TransactionDto>.SuccessResponse(
            dto,
            "Transaction created successfully.");
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
}