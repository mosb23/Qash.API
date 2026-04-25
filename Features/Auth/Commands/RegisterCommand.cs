using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.DTOs;

namespace Qash.API.Features.Auth.Commands;

public class RegisterCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}