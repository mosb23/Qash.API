using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.Auth.Commands;

public class LogoutCommand : IRequest<ApiResponse<string>>
{
    public string RefreshToken { get; set; } = string.Empty;
}