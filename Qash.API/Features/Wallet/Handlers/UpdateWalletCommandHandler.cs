using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.Commands;
using Qash.API.Features.Wallet.DTOs;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Wallet.Handlers;

public class UpdateWalletCommandHandler : IRequestHandler<UpdateWalletCommand, ApiResponse<WalletDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public UpdateWalletCommandHandler(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<ApiResponse<WalletDto>> Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets
        .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

    if (wallet is null)
    {
      return ApiResponse<WalletDto>.FailResponse("Update wallet failed.", ["Wallet was not found."]);
    }

    wallet.Name = request.Name.Trim();
    wallet.Currency = request.Currency.Trim().ToUpper();
    wallet.Balance = request.Balance;

    await _context.SaveChangesAsync(cancellationToken);

    var dto = _mapper.Map<WalletDto>(wallet);

    return ApiResponse<WalletDto>.SuccessResponse(dto, "Wallet updated successfully.");
  }
}