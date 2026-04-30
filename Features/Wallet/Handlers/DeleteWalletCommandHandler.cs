using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.Commands;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Wallet.Handlers;

public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand, ApiResponse<string>>
{
  private readonly ApplicationDbContext _context;

  public DeleteWalletCommandHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<ApiResponse<string>> Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets
        .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

    if (wallet is null)
    {
      return ApiResponse<string>.FailResponse("Delete wallet failed.", ["Wallet was not found."]);
    }

    var transactions = await _context.Transactions
        .Where(x => x.WalletId == wallet.Id)
        .ToListAsync(cancellationToken);

    _context.Transactions.RemoveRange(transactions);

    _context.Wallets.Remove(wallet);
    await _context.SaveChangesAsync(cancellationToken);

    return ApiResponse<string>.SuccessResponse("Wallet deleted", "Wallet deleted successfully.");
  }
}