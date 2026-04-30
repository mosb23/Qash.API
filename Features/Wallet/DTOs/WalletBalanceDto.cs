using System;

namespace Qash.API.Features.Wallet.DTOs;

public class WalletBalanceDto
{
  public Guid WalletId { get; set; }

  public decimal Balance { get; set; }
}