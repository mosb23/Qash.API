using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using System;

namespace Qash.API.Features.Wallet.Queries;

public class GetWalletByIdQuery : IRequest<ApiResponse<WalletDto>>
{
  public Guid UserId { get; set; }

  public Guid WalletId { get; set; }

  public GetWalletByIdQuery(Guid userId, Guid walletId)
  {
    UserId = userId;
    WalletId = walletId;
  }
}