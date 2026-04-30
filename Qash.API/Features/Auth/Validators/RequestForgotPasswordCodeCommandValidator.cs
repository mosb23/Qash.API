using FluentValidation;
using Qash.API.Features.Auth.Commands;

namespace Qash.API.Features.Auth.Validators;

public class RequestForgotPasswordCodeCommandValidator : AbstractValidator<RequestForgotPasswordCodeCommand>
{
    public RequestForgotPasswordCodeCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10,15}$")
            .WithMessage("Invalid phone number.");
    }
}