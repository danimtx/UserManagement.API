using FluentValidation;
using System;
using UserManagement.Application.DTOs.User.Profile;

namespace UserManagement.Application.Validators
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FotoPerfilUrl)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.FotoPerfilUrl))
                .WithMessage("El formato de la URL de la foto de perfil no es válido.");

            // Se podrían añadir más reglas para otros campos como Celular, etc.
        }
    }
}
