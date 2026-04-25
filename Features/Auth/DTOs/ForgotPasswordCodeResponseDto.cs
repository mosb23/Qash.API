namespace Qash.API.Features.Auth.DTOs;

public class ForgotPasswordCodeResponseDto
{
    public string PhoneNumber { get; set; } = string.Empty;

    public string VerificationCode { get; set; } = string.Empty;

    public string Note { get; set; } =
        "Demo only. In production, this code should be sent by SMS OTP service.";
}