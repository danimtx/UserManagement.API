using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Auth;

namespace UserManagement.Application.Validators
{
    public class RegisterCompanyValidator : AbstractValidator<RegisterCompanyDto>
    {
        public RegisterCompanyValidator()
        {
            RuleFor(x => x.EmailEmpresa).NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .Equal(x => x.ConfirmPassword).WithMessage("Las contraseñas no coinciden.");

            RuleFor(x => x.RazonSocial).NotEmpty().WithMessage("La Razón Social es obligatoria.");
            RuleFor(x => x.Nit).NotEmpty().WithMessage("El NIT es obligatorio.");

            RuleFor(x => x.Sucursales)
                .Must(x => x != null && x.Count > 0)
                .WithMessage("Debe registrar al menos una sucursal (Oficina Central).");

            RuleForEach(x => x.Sucursales).ChildRules(sucursal => {
                sucursal.RuleFor(s => s.Nombre).NotEmpty();
                sucursal.RuleFor(s => s.Direccion).NotEmpty();
                sucursal.RuleFor(s => s.Departamento).NotEmpty();
            });
        }
    }
}
