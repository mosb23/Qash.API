using Qash.API.Domain.Common;

namespace Qash.API.Domain.Entities;

public class SavingGoal : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateTime Deadline { get; set; }
}
