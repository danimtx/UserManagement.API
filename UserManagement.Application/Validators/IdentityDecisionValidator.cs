using FluentValidation;
using UserManagement.Application.DTOs.Admin;

namespace UserManagement.Application.Validators
{
    public class IdentityDecisionValidator : AbstractValidator<IdentityDecisionDto>
    {
        public IdentityDecisionValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
