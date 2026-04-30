using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Domain.Enums;
using Qash.API.Features.Categories.Commands;
using Qash.API.Features.Categories.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryType? type)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new GetCategoriesQuery(userId.Value, type));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        command.UserId = userId.Value;

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        command.Id = id;
        command.UserId = userId.Value;

        var response = await _mediator.Send(command);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var response = await _mediator.Send(new DeleteCategoryCommand(id, userId.Value));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
            return null;

        return userId;
    }
}