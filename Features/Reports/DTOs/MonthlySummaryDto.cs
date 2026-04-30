namespace Qash.API.Features.Reports.DTOs;

public class MonthlySummaryDto
{
    public decimal TotalIncome { get; set; }

    public decimal TotalExpenses { get; set; }

    public decimal NetBalance { get; set; }

    public int TransactionCount { get; set; }
}