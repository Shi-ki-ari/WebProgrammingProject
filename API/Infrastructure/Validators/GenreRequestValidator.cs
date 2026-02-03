using FluentValidation;
using API.Infrastructure.RequestDTOs.Genres;
using Common.Persistence;

namespace API.Infrastructure.Validators;

public class GenreRequestValidator : AbstractValidator<GenreRequest>
{
    public GenreRequestValidator(AppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .Must(name => !context.Genres.Any(g => g.Name == name))
            .WithMessage("Genre with this name already exists.");
    }
}
