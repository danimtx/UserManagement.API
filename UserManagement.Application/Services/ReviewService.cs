using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Review;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUserRepository _userRepository;

        public ReviewService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateReviewAsync(string authorId, CreateReviewDto dto)
        {
            if (authorId == dto.RecipientId)
            {
                throw new Exception("Un usuario no puede dejarse una reseña a sí mismo.");
            }

            var hasAlreadyReviewed = await _userRepository.HasUserReviewedContextAsync(authorId, dto.RecipientId, dto.ContextoId);
            if (hasAlreadyReviewed)
            {
                throw new Exception("Ya has dejado una reseña para este perfil/tag.");
            }

            var recipient = await _userRepository.GetByIdAsync(dto.RecipientId);
            if (recipient == null)
            {
                throw new Exception("El destinatario de la reseña no fue encontrado.");
            }

            if (recipient.TipoUsuario == UserType.Personal.ToString())
            {
                var personalProfile = recipient.DatosPersonales;
                if (personalProfile == null) throw new Exception("El perfil personal del destinatario no existe.");

                var tag = personalProfile.Tags.FirstOrDefault(t => t.Nombre.Equals(dto.ContextoId, StringComparison.OrdinalIgnoreCase));
                if (tag == null) throw new Exception($"El tag '{dto.ContextoId}' no fue encontrado en el perfil del usuario.");
                if (tag.Estado != TagStatus.Activo) throw new Exception("Solo se pueden dejar reseñas en tags activos.");

                // Actualizar rating del tag
                tag.CalificacionPromedio = ((tag.CalificacionPromedio * tag.TotalResenas) + dto.Rating) / (tag.TotalResenas + 1);
                tag.TotalResenas++;
            }
            else if (recipient.TipoUsuario == UserType.Empresa.ToString())
            {
                var companyProfile = recipient.DatosEmpresa;
                if (companyProfile == null) throw new Exception("El perfil de empresa del destinatario no existe.");

                var profile = companyProfile.PerfilesComerciales.FirstOrDefault(p => p.Id == dto.ContextoId);
                if (profile == null) throw new Exception($"El perfil comercial con ID '{dto.ContextoId}' no fue encontrado.");
                if (profile.Estado != CommercialProfileStatus.Activo) throw new Exception("Solo se pueden dejar reseñas en perfiles comerciales activos.");

                // Actualizar rating del perfil comercial
                profile.CalificacionPromedio = ((profile.CalificacionPromedio * profile.TotalResenas) + dto.Rating) / (profile.TotalResenas + 1);
                profile.TotalResenas++;
            }
            else
            {
                throw new Exception("El tipo de usuario destinatario no es válido para recibir reseñas.");
            }

            var review = new Review
            {
                AuthorId = authorId,
                RecipientId = dto.RecipientId,
                ContextoId = dto.ContextoId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                Timestamp = DateTime.UtcNow
            };

            // Guardar la nueva reseña y actualizar el perfil del usuario calificado
            await _userRepository.AddReviewAsync(review);
            await _userRepository.UpdateAsync(recipient);
        }
    }
}
