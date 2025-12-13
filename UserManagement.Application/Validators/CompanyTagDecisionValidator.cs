using FluentValidation;
using UserManagement.Application.DTOs.Admin;

namespace UserManagement.Application.Validators
{
    public class CompanyTagDecisionValidator : AbstractValidator<CompanyTagDecisionDto>
    {
        public CompanyTagDecisionValidator()
        {
            RuleFor(x => x.CompanyUserId).NotEmpty();
            RuleFor(x => x.CommercialProfileId).NotEmpty();
        }
    }
}
