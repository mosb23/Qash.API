using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Wallet.Queries;

public class GetWalletsQuery : IRequest<ApiResponse<List<WalletDto>>>
{
  public Guid UserId { get; set; }

  public GetWalletsQuery(Guid userId)
  {
    UserId = userId;
  }
}