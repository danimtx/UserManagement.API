using FluentValidation;
using UserManagement.Application.DTOs.Review;

namespace UserManagement.Application.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.RecipientId).NotEmpty();
            RuleFor(x => x.ContextoId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("La calificación debe estar entre 1 y 5.");
        }
    }
}
