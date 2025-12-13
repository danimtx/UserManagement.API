using FluentValidation;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.Validators
{
    public class RequestCommercialProfileValidator : AbstractValidator<RequestCommercialProfileDto>
    {
        public RequestCommercialProfileValidator()
        {
            RuleFor(x => x.NombreComercial).NotEmpty().WithMessage("El nombre comercial es requerido.");
            RuleFor(x => x.Documentos).NotEmpty().WithMessage("Debe proporcionar al menos un documento.");
        }
    }
}
