using FluentValidation;
using UserManagement.Application.DTOs.Admin;

namespace UserManagement.Application.Validators
{
    public class PersonalTagDecisionValidator : AbstractValidator<PersonalTagDecisionDto>
    {
        public PersonalTagDecisionValidator()
        {
            RuleFor(x => x.PersonalUserId).NotEmpty();
            RuleFor(x => x.TagName).NotEmpty();
        }
    }
}
