using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.Auth.Commands;

public class ResetForgotPasswordCommand : IRequest<ApiResponse<string>>
{
    public string PhoneNumber { get; set; } = string.Empty;

    public string VerificationCode { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}