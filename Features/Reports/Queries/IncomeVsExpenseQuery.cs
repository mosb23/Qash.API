using MediatR;
using Qash.API.Features.Reports.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Reports.Queries;

public class IncomeVsExpenseQuery : IRequest<List<IncomeVsExpenseDto>>
{
    public Guid UserId { get; set; }

    public int Year { get; set; }

    public IncomeVsExpenseQuery(Guid userId, int year)
    {
        UserId = userId;
        Year = year;
    }
}