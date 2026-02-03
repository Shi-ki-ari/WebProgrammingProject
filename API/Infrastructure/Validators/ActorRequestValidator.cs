using FluentValidation;
using API.Infrastructure.RequestDTOs.Actors;
using Common.Persistence;

namespace API.Infrastructure.Validators;

public class ActorRequestValidator : AbstractValidator<ActorRequest>
{
    public ActorRequestValidator(AppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .Must(name => !context.Actors.Any(a => a.Name == name))
            .WithMessage("Actor with this name already exists.");
    }
}
