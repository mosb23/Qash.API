using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Qash.API.Features.Wallet.Commands;
using Qash.API.Features.Wallet.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Qash.API.Controllers;

[ApiController]
[Authorize]
[Route("api/wallets")]
public class WalletController : ControllerBase
{
  private readonly IMediator _mediator;

  public WalletController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateWallet([FromBody] CreateWalletCommand command)
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    command.UserId = userId.Value;

    var response = await _mediator.Send(command);
    return response.Success ? Ok(response) : BadRequest(response);
  }

  [HttpGet]
  public async Task<IActionResult> GetWallets()
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    var response = await _mediator.Send(new GetWalletsQuery(userId.Value));
    return response.Success ? Ok(response) : BadRequest(response);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetWalletById([FromRoute] Guid id)
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    var response = await _mediator.Send(new GetWalletByIdQuery(userId.Value, id));
    return response.Success ? Ok(response) : NotFound(response);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateWallet([FromRoute] Guid id, [FromBody] UpdateWalletCommand command)
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    command.UserId = userId.Value;
    command.WalletId = id;

    var response = await _mediator.Send(command);
    return response.Success ? Ok(response) : BadRequest(response);
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> DeleteWallet([FromRoute] Guid id)
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    var response = await _mediator.Send(new DeleteWalletCommand
    {
      UserId = userId.Value,
      WalletId = id
    });

    return response.Success ? Ok(response) : BadRequest(response);
  }

  [HttpGet("{id:guid}/balance")]
  public async Task<IActionResult> GetWalletBalance([FromRoute] Guid id)
  {
    var userId = GetCurrentUserId();

    if (userId is null)
      return Unauthorized();

    var response = await _mediator.Send(new GetWalletBalanceQuery(userId.Value, id));
    return response.Success ? Ok(response) : NotFound(response);
  }

  private Guid? GetCurrentUserId()
  {
    var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!Guid.TryParse(userIdValue, out var userId))
      return null;

    return userId;
  }
}