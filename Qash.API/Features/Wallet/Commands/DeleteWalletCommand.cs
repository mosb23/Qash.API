using MediatR;
using Qash.API.Common.Responses;
using System;

namespace Qash.API.Features.Wallet.Commands;

public class DeleteWalletCommand : IRequest<ApiResponse<string>>
{
  public Guid UserId { get; set; }

  public Guid WalletId { get; set; }
}