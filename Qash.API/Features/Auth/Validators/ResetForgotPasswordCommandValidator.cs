using FluentValidation;
using Qash.API.Features.Auth.Commands;

namespace Qash.API.Features.Auth.Validators;

public class ResetForgotPasswordCommandValidator : AbstractValidator<ResetForgotPasswordCommand>
{
    public ResetForgotPasswordCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10,15}$")
            .WithMessage("Invalid phone number.");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .Equal("00000")
            .WithMessage("Invalid verification code.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain number.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}