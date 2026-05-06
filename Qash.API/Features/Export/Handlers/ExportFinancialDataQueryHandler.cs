using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Qash.API.Domain.Enums;
using Qash.API.Features.Export.DTOs;
using Qash.API.Features.Export.Queries;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Export.Handlers;

public class ExportFinancialDataQueryHandler : IRequestHandler<ExportFinancialDataQuery, ExportFileResult>
{
    private readonly ApplicationDbContext _context;

    public ExportFinancialDataQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExportFileResult> Handle(ExportFinancialDataQuery request, CancellationToken cancellationToken)
    {
        var from = NormalizeStartUtc(request.FromUtc);
        var toExclusive = NormalizeEndExclusiveUtc(request.ToUtc);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(x => x.Wallet)
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == request.UserId)
            .Where(x => x.TransactionDate >= from && x.TransactionDate < toExclusive)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync(cancellationToken);

        var rows = transactions
            .Select(x => new ExportRow(
                x.TransactionDate,
                x.TransactionType.ToString(),
                x.Amount,
                x.Wallet.Name,
                x.Category.Name,
                x.Description))
            .ToList();

        return request.Format switch
        {
            ExportFormat.Pdf => BuildPdf(from, toExclusive, rows),
            _ => BuildCsv(rows)
        };
    }

    private static DateTime NormalizeStartUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    private static DateTime NormalizeEndExclusiveUtc(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
    }

    private static ExportFileResult BuildCsv(List<ExportRow> rows)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, new UTF8Encoding(true), leaveOpen: true))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteField("DateUtc");
            csv.WriteField("Type");
            csv.WriteField("Amount");
            csv.WriteField("Wallet");
            csv.WriteField("Category");
            csv.WriteField("Description");
            csv.NextRecord();

            foreach (var row in rows)
            {
                csv.WriteField(row.DateUtc.ToString("o", CultureInfo.InvariantCulture));
                csv.WriteField(row.Type);
                csv.WriteField(row.Amount.ToString(CultureInfo.InvariantCulture));
                csv.WriteField(row.Wallet);
                csv.WriteField(row.Category);
                csv.WriteField(row.Description);
                csv.NextRecord();
            }
        }

        var fileName = $"qash-transactions-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

        return new ExportFileResult
        {
            Content = stream.ToArray(),
            ContentType = "text/csv; charset=utf-8",
            FileName = fileName
        };
    }

    private static ExportFileResult BuildPdf(DateTime from, DateTime toExclusive, List<ExportRow> rows)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(24);
                page.Size(PageSizes.A4);

                page.Header().Column(column =>
                {
                    column.Item().Text("Qash transaction export").SemiBold().FontSize(18);
                    column.Item().PaddingTop(4).Text(
                        $"Range (UTC): {from:yyyy-MM-dd} – {toExclusive.AddDays(-1):yyyy-MM-dd}");
                });

                page.Content().PaddingVertical(12).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(1.2f);
                        columns.RelativeColumn(2);
                    });

                    static IContainer HeaderCell(IContainer cell) =>
                        cell.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Medium);

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Date (UTC)");
                        header.Cell().Element(HeaderCell).Text("Type");
                        header.Cell().Element(HeaderCell).Text("Amount");
                        header.Cell().Element(HeaderCell).Text("Wallet");
                        header.Cell().Element(HeaderCell).Text("Category");
                        header.Cell().Element(HeaderCell).Text("Note");
                    });

                    foreach (var row in rows)
                    {
                        table.Cell().PaddingVertical(3).Text(row.DateUtc.ToString("yyyy-MM-dd HH:mm"));
                        table.Cell().PaddingVertical(3).Text(row.Type);
                        table.Cell().PaddingVertical(3).Text(row.Amount.ToString("0.00", CultureInfo.InvariantCulture));
                        table.Cell().PaddingVertical(3).Text(row.Wallet);
                        table.Cell().PaddingVertical(3).Text(row.Category);
                        table.Cell().PaddingVertical(3).Text(row.Description);
                    }
                });
            });
        });

        var bytes = document.GeneratePdf();
        var fileName = $"qash-transactions-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";

        return new ExportFileResult
        {
            Content = bytes,
            ContentType = "application/pdf",
            FileName = fileName
        };
    }

    private sealed record ExportRow(
        DateTime DateUtc,
        string Type,
        decimal Amount,
        string Wallet,
        string Category,
        string Description);
}
