using FluentValidation;
using API.Infrastructure.RequestDTOs.Reviews;

namespace API.Infrastructure.Validators;

public class ReviewRequestValidator : AbstractValidator<ReviewRequest>
{
    public ReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters.");
    }
}
