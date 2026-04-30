using Qash.API.Domain.Common;
using System;
using System.Collections.Generic;

namespace Qash.API.Domain.Entities;

public class Wallet : BaseEntity
{
  public string Name { get; set; } = string.Empty;

  public string Currency { get; set; } = "USD";

  public decimal Balance { get; set; }

  public Guid ApplicationUserId { get; set; }

  public ApplicationUser ApplicationUser { get; set; } = null!;

  public List<Transaction> Transactions { get; set; } = [];
}