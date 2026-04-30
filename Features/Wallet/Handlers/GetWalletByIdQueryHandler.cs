using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using Qash.API.Features.Wallet.Queries;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Wallet.Handlers;

public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, ApiResponse<WalletDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetWalletByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<ApiResponse<WalletDto>> Handle(GetWalletByIdQuery request, CancellationToken cancellationToken)
  {
    var wallet = await _context.Wallets
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.ApplicationUserId == request.UserId, cancellationToken);

    if (wallet is null)
    {
      return ApiResponse<WalletDto>.FailResponse("Wallet not found.", ["Wallet was not found."]);
    }

    var dto = _mapper.Map<WalletDto>(wallet);

    return ApiResponse<WalletDto>.SuccessResponse(dto, "Wallet retrieved successfully.");
  }
}