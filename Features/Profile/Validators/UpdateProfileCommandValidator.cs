using FluentValidation;
using Qash.API.Features.Profile.Commands;

namespace Qash.API.Features.Profile.Validators;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
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
    }

    private bool BeValidDemoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        email = email.Trim().ToLower();
        return email.Contains("@") && email.EndsWith(".com");
    }
}