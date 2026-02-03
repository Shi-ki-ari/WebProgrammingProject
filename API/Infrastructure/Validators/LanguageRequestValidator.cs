using FluentValidation;
using API.Infrastructure.RequestDTOs.Languages;
using Common.Persistence;

namespace API.Infrastructure.Validators;

public class LanguageRequestValidator : AbstractValidator<LanguageRequest>
{
    public LanguageRequestValidator(AppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .Must(name => !context.Languages.Any(l => l.Name == name))
            .WithMessage("Language with this name already exists.");
    }
}
