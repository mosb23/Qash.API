using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using Qash.API.Features.Wallet.Queries;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Wallet.Handlers;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, ApiResponse<WalletBalanceDto>>
{
  private readonly ApplicationDbContext _context;

  public GetWalletBalanceQueryHandler(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<ApiResponse<WalletBalanceDto>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

    if (wallet is null)
    {
      return ApiResponse<WalletBalanceDto>.FailResponse("Wallet not found.", ["Wallet was not found."]);
    }

    return ApiResponse<WalletBalanceDto>.SuccessResponse(
        new WalletBalanceDto
        {
          WalletId = wallet.Id,
          Balance = wallet.Balance
        },
        "Wallet balance retrieved successfully.");
  }
}