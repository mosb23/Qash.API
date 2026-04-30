using MediatR;
using Qash.API.Features.Reports.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Reports.Queries;

public class CategoryBreakdownQuery : IRequest<List<CategoryBreakdownDto>>
{
    public Guid UserId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public CategoryBreakdownQuery(Guid userId, int year, int month)
    {
        UserId = userId;
        Year = year;
        Month = month;
    }
}