using System;

namespace Qash.API.Features.Reports.DTOs;

public class SpendingTrendDto
{
    public DateTime Date { get; set; }

    public decimal TotalExpenses { get; set; }
}