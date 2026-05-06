using MediatR;
using Qash.API.Features.Reports.DTOs;

namespace Qash.API.Features.Reports.Queries;

public class DateRangeSummaryQuery : IRequest<DateRangeSummaryDto>
{
    public DateRangeSummaryQuery(Guid userId, DateTime fromUtc, DateTime toUtc)
    {
        UserId = userId;
        FromUtc = fromUtc;
        ToUtc = toUtc;
    }

    public Guid UserId { get; }

    public DateTime FromUtc { get; }

    public DateTime ToUtc { get; }
}
