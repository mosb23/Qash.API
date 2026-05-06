using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Domain.Enums;
using Qash.API.Features.Export.Queries;
using System.Security.Claims;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/export")]
public class ExportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<ExportFinancialDataQuery> _exportValidator;

    public ExportController(IMediator mediator, IValidator<ExportFinancialDataQuery> exportValidator)
    {
        _mediator = mediator;
        _exportValidator = exportValidator;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> ExportTransactions(
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        [FromQuery] ExportFormat format)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var query = new ExportFinancialDataQuery(userId.Value, fromUtc, toUtc, format);
        var validation = await _exportValidator.ValidateAsync(query);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray() });

        var file = await _mediator.Send(query);
        return File(file.Content, file.ContentType, file.FileName);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
