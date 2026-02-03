using FluentValidation;
using API.Infrastructure.RequestDTOs.Users;
using Common.Persistence;

namespace API.Infrastructure.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(AppDbContext context)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.")
            .Must(username => !context.Users.Any(u => u.Username == username))
            .WithMessage("Username already exists.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
            .Must(email => !context.Users.Any(u => u.Email == email))
            .WithMessage("Email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 50).WithMessage("Password must be between 6 and 50 characters.");
    }
}
