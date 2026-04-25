using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.Auth.Commands;

public class VerifyPhoneCommand : IRequest<ApiResponse<string>>
{
    public string PhoneNumber { get; set; } = string.Empty;

    public string VerificationCode { get; set; } = string.Empty;
}