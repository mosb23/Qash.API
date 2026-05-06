namespace Qash.API.Features.Budgets.DTOs;

public class BudgetDto
{
    public Guid BudgetId { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Amount { get; set; }
}
