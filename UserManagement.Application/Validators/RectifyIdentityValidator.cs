using FluentValidation;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.Validators
{
    public class RectifyIdentityValidator : AbstractValidator<RectifyIdentityDto>
    {
        public RectifyIdentityValidator()
        {
            RuleFor(x => x.RazonSocial).NotEmpty().WithMessage("La razón social es requerida.");
            RuleFor(x => x.Nit).NotEmpty().WithMessage("El NIT es requerido.");
        }
    }
}
