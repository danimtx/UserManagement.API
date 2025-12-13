using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RequestTagAsync(string userId, RequestTagDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Usuario no encontrado.");
            if (user.TipoUsuario != UserType.Personal.ToString()) throw new Exception("Esta función solo está disponible para usuarios personales.");
            if (user.DatosPersonales == null) throw new Exception("El perfil personal no está configurado.");

            // Validar que no exista ya un tag con el mismo nombre
            if (user.DatosPersonales.Tags.Any(t => t.Nombre.Equals(dto.TagNombre, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Ya existe una solicitud para el tag '{dto.TagNombre}'.");
            }

            // Lógica de validación de documentos según el plan
            if (dto.TagNombre.Equals("Vendedor", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.Evidencias.Any(e => e.Tipo.Contains("FacturaServicioBasico")))
                {
                    throw new Exception("Para solicitar el tag 'Vendedor', se requiere una factura de servicio básico.");
                }
            }
            else // Oficios
            {
                if (dto.EsEmpirico && !dto.Evidencias.Any(e => e.Tipo.Contains("FotoTaller") || e.Tipo.Contains("FotoTrabajo")))
                {
                    throw new Exception("Para oficios empíricos, se requieren fotos del taller o trabajos realizados.");
                }
                if (!dto.EsEmpirico && !dto.Evidencias.Any(e => e.Tipo.Contains("Titulo") || e.Tipo.Contains("Certificado")))
                {
                    throw new Exception("Para oficios no empíricos, se requiere un título o certificado.");
                }
            }

            var newTag = new Tag
            {
                Nombre = dto.TagNombre,
                EsEmpirico = dto.EsEmpirico,
                Estado = TagStatus.Pendiente,
                Evidencias = dto.Evidencias.Select(e => new UploadedDocument
                {
                    TipoDocumento = e.Tipo,
                    UrlArchivo = e.Url,
                    FechaSubida = DateTime.UtcNow,
                    EstadoValidacion = "Pendiente"
                }).ToList()
            };

            user.DatosPersonales.Tags.Add(newTag);
            await _userRepository.UpdateAsync(user);
        }
    }
}
