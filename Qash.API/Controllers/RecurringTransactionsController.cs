using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Features.RecurringTransactions.Commands;
using Qash.API.Features.RecurringTransactions.Queries;
using System.Security.Claims;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/recurring-transactions")]
public class RecurringTransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecurringTransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new GetRecurringTransactionsQuery(userId.Value));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new GetRecurringTransactionByIdQuery(userId.Value, id));
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecurringTransactionCommand command)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        command.UserId = userId.Value;
        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecurringTransactionCommand command)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        command.UserId = userId.Value;
        command.RecurringTransactionId = id;
        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new DeleteRecurringTransactionCommand
        {
            UserId = userId.Value,
            RecurringTransactionId = id
        });

        return response.Success ? Ok(response) : BadRequest(response);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
