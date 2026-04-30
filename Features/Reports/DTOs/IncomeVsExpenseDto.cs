namespace Qash.API.Features.Reports.DTOs;

public class IncomeVsExpenseDto
{
    public int Month { get; set; }

    public decimal Income { get; set; }

    public decimal Expenses { get; set; }
}