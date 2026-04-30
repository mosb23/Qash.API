using FluentValidation;
using Qash.API.Domain.Enums;
using Qash.API.Features.Categories.Commands;

namespace Qash.API.Features.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Type)
            .Must(x => x == CategoryType.Income || x == CategoryType.Expense)
            .WithMessage("Category type must be Income or Expense.");

        RuleFor(x => x.Icon)
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .MaximumLength(20);
    }
}