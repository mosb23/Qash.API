namespace Qash.API.Features.Reports.DTOs;

public class CategoryBreakdownDto
{
    public string CategoryId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}