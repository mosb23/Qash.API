using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Insights.DTOs;

namespace Qash.API.Features.Insights.Queries;

public class GetInsightsQuery : IRequest<ApiResponse<InsightsResponseDto>>
{
    public GetInsightsQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
