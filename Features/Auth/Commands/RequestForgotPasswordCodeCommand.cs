using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.DTOs;

namespace Qash.API.Features.Auth.Commands;

public class RequestForgotPasswordCodeCommand : IRequest<ApiResponse<ForgotPasswordCodeResponseDto>>
{
    public string PhoneNumber { get; set; } = string.Empty;
}