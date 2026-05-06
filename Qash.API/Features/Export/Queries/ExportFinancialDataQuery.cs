using MediatR;
using Qash.API.Domain.Enums;
using Qash.API.Features.Export.DTOs;

namespace Qash.API.Features.Export.Queries;

public class ExportFinancialDataQuery : IRequest<ExportFileResult>
{
    public ExportFinancialDataQuery(Guid userId, DateTime fromUtc, DateTime toUtc, ExportFormat format)
    {
        UserId = userId;
        FromUtc = fromUtc;
        ToUtc = toUtc;
        Format = format;
    }

    public Guid UserId { get; }

    public DateTime FromUtc { get; }

    public DateTime ToUtc { get; }

    public ExportFormat Format { get; }
}
