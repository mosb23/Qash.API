using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.DTOs;

namespace Qash.API.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}