using Qash.API.Domain.Common;
using Qash.API.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Qash.API.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }

    public string? Icon { get; set; }

    public string? Color { get; set; }

    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = [];
}