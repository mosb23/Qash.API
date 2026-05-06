using FluentValidation;
using Qash.API.Domain.Enums;
using Qash.API.Features.Export.Queries;

namespace Qash.API.Features.Export.Validators;

public class ExportFinancialDataQueryValidator : AbstractValidator<ExportFinancialDataQuery>
{
    public ExportFinancialDataQueryValidator()
    {
        RuleFor(x => x.FromUtc)
            .LessThanOrEqualTo(x => x.ToUtc)
            .WithMessage("From date must be before or equal to To date.");

        RuleFor(x => x)
            .Must(x => (x.ToUtc - x.FromUtc).TotalDays <= 732)
            .WithMessage("Date range cannot exceed two years.");

        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("Format must be Csv or Pdf.");
    }
}
