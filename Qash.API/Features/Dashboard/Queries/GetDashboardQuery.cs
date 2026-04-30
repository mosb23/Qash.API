using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Dashboard.DTOs;
using System;

namespace Qash.API.Features.Dashboard.Queries;

public class GetDashboardQuery : IRequest<ApiResponse<DashboardDto>>
{
    public Guid UserId { get; set; }

    public GetDashboardQuery(Guid userId)
    {
        UserId = userId;
    }
}