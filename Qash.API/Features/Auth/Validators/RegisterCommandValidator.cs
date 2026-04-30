using FluentValidation;
using Qash.API.Features.Auth.Commands;

namespace Qash.API.Features.Auth.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(BeValidDemoEmail)
            .WithMessage("Email must contain @ and end with .com");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10,15}$")
            .WithMessage("Invalid phone number.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain number.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }

    private bool BeValidDemoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        email = email.Trim().ToLower();
        return email.Contains("@") && email.EndsWith(".com");
    }
}