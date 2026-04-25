using FluentValidation;
using Qash.API.Features.Auth.Commands;

namespace Qash.API.Features.Auth.Validators;

public class VerifyPhoneCommandValidator : AbstractValidator<VerifyPhoneCommand>
{
	public VerifyPhoneCommandValidator()
	{
		RuleFor(x => x.PhoneNumber)
			.NotEmpty()
			.Matches(@"^[0-9]{10,15}$")
			.WithMessage("Invalid phone number.");

		RuleFor(x => x.VerificationCode)
			.NotEmpty()
			.Equal("00000")
			.WithMessage("Invalid verification code.");
	}
}