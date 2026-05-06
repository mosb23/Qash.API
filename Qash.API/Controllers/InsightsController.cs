using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Features.Insights.Queries;
using System.Security.Claims;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/insights")]
public class InsightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InsightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetInsights()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new GetInsightsQuery(userId.Value));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
