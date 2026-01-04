using FluentValidation;
using UserManagement.Application.DTOs.Company.Employees;

namespace UserManagement.Application.Validators
{
    public class ResetEmployeePasswordDtoValidator : AbstractValidator<ResetEmployeePasswordDto>
    {
        public ResetEmployeePasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La nueva contraseña no puede estar vacía.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
        }
    }
}
