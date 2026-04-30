using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.DTOs;
using System;

namespace Qash.API.Features.Wallet.Commands;

public class CreateWalletCommand : IRequest<ApiResponse<WalletDto>>
{
  public Guid UserId { get; set; }

  public string Name { get; set; } = string.Empty;

  public string Currency { get; set; } = string.Empty;

  public decimal InitialBalance { get; set; }
}