using MediatR;
using Qash.API.Features.Reports.DTOs;
using System;

namespace Qash.API.Features.Reports.Queries;

public class MonthlySummaryQuery : IRequest<MonthlySummaryDto>
{
    public Guid UserId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public MonthlySummaryQuery(Guid userId, int year, int month)
    {
        UserId = userId;
        Year = year;
        Month = month;
    }
}