using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Auth;

namespace UserManagement.Application.Validators
{
    public class RegisterPersonalValidator : AbstractValidator<RegisterPersonalDto>
    {
        public RegisterPersonalValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("El formato del correo no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .Equal(x => x.ConfirmPassword).WithMessage("Las contraseñas no coinciden.");

            RuleFor(x => x.Nombres).NotEmpty();
            RuleFor(x => x.ApellidoPaterno).NotEmpty();
            RuleFor(x => x.CI).NotEmpty().WithMessage("El Carnet de Identidad es obligatorio.");

            RuleFor(x => x.FechaNacimiento)
                .LessThan(DateTime.Now.AddYears(-18)).WithMessage("El usuario debe ser mayor de edad.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 letras.")
                .Matches("^[a-zA-Z0-9._]+$").WithMessage("El usuario solo puede contener letras, números, puntos o guiones bajos.");
        }
    }
}
