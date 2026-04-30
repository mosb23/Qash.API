using MediatR;
using Qash.API.Features.Reports.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Reports.Queries;

public class SpendingTrendQuery : IRequest<List<SpendingTrendDto>>
{
    public Guid UserId { get; set; }

    public int Days { get; set; }

    public SpendingTrendQuery(Guid userId, int days)
    {
        UserId = userId;
        Days = days;
    }
}