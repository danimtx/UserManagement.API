using FluentValidation;
using UserManagement.Application.DTOs.User;

namespace UserManagement.Application.Validators
{
    public class RequestTagValidator : AbstractValidator<RequestTagDto>
    {
        public RequestTagValidator()
        {
            RuleFor(x => x.TagNombre).NotEmpty().WithMessage("El nombre del tag es requerido.");
            RuleFor(x => x.Evidencias).NotEmpty().WithMessage("Debe proporcionar al menos un documento de evidencia.");
        }
    }
}
