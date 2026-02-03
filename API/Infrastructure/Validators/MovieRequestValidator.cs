using FluentValidation;
using API.Infrastructure.RequestDTOs.Movies;
using Common.Persistence;

namespace API.Infrastructure.Validators;

public class MovieRequestValidator : AbstractValidator<MovieRequest>
{
    public MovieRequestValidator(AppDbContext context)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.")
            .Must(title => !context.Movies.Any(m => m.Title == title))
            .WithMessage("Movie with this title already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.ReleaseYear)
            .InclusiveBetween(1888, DateTime.Now.Year + 1)
            .WithMessage($"Release year must be between 1888 and {DateTime.Now.Year + 1}.");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("At least one genre is required.")
            .Must(ids => ids != null && ids.All(id => id > 0))
            .WithMessage("All genre IDs must be greater than 0.")
            .Must(ids => ids != null && ids.Distinct().Count() == ids.Count)
            .WithMessage("Genre IDs must be unique.");

        RuleFor(x => x.ActorIds)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All actor IDs must be greater than 0.")
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Actor IDs must be unique.");

        RuleFor(x => x.LanguageIds)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All language IDs must be greater than 0.")
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Language IDs must be unique.");
    }
}
