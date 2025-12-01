using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Company;

namespace UserManagement.Application.Validators
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.PasswordTemporal).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Nombres).NotEmpty();
            RuleFor(x => x.CI).NotEmpty();
            RuleFor(x => x.AreaTrabajo).NotEmpty().WithMessage("Debe asignar un área de trabajo.");

            RuleFor(x => x.Permisos)
                .Must(p => p != null && p.Count > 0)
                .WithMessage("Debe asignar permisos para al menos un módulo.");
        }
    }
}
