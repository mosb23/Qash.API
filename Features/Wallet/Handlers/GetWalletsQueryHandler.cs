using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using Qash.API.Features.Wallet.Queries;
using Qash.API.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Wallet.Handlers;

public class GetWalletsQueryHandler : IRequestHandler<GetWalletsQuery, ApiResponse<List<WalletDto>>>
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public GetWalletsQueryHandler(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<ApiResponse<List<WalletDto>>> Handle(GetWalletsQuery request, CancellationToken cancellationToken)
  {
    var wallets = await _context.Wallets
        .AsNoTracking()
        .Where(x => x.ApplicationUserId == request.UserId)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync(cancellationToken);

    var dto = _mapper.Map<List<WalletDto>>(wallets);

    return ApiResponse<List<WalletDto>>.SuccessResponse(dto, "Wallets retrieved successfully.");
  }
}