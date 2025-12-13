using FluentValidation;
using UserManagement.Application.DTOs.Admin;

namespace UserManagement.Application.Validators
{
    public class ModuleDecisionValidator : AbstractValidator<ModuleDecisionDto>
    {
        public ModuleDecisionValidator()
        {
            RuleFor(x => x.CompanyUserId).NotEmpty();
            RuleFor(x => x.CommercialProfileId).NotEmpty();
        }
    }
}
