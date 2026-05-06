namespace Qash.API.Features.Export.DTOs;

public class ExportFileResult
{
    public byte[] Content { get; set; } = [];

    public string ContentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}
