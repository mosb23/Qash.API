using System;

namespace Qash.API.Features.Wallet.DTOs;

public class WalletDto
{
  public Guid WalletId { get; set; }

  public string Name { get; set; } = string.Empty;

  public string Currency { get; set; } = string.Empty;

  public decimal Balance { get; set; }

  public Guid UserId { get; set; }
}