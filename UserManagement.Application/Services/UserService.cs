using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.DTOs.User.Profile;
using UserManagement.Application.DTOs.Public;
using UserManagement.Application.DTOs.Shared; // Added
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

            if (user.DatosPersonales.Tags.Any(t => t.Nombre.Equals(dto.TagNombre, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Ya existe una solicitud para el tag '{dto.TagNombre}'.");
            }
            
            if (dto.TagNombre.Equals("Vendedor", StringComparison.OrdinalIgnoreCase))
            {
                if (!dto.Evidencias.Any(e => e.Tipo.Contains("FacturaServicioBasico")))
                {
                    throw new Exception("Para solicitar el tag 'Vendedor', se requiere una factura de servicio básico.");
                }
            }
            else 
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

        public async Task RequestModuleAsync(string userId, RequestPersonalModuleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Usuario no encontrado.");
            if (user.TipoUsuario != UserType.Personal.ToString()) throw new Exception("Esta función solo está disponible para usuarios personales.");
            if (user.DatosPersonales == null) throw new Exception("El perfil personal no está configurado.");

            if (user.DatosPersonales.SolicitudesModulos.Any(m => m.NombreModulo.Equals(dto.NombreModulo, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Ya existe una solicitud para el módulo '{dto.NombreModulo}'.");
            }

            var newModuleRequest = new ModuleRequest
            {
                NombreModulo = dto.NombreModulo,
                Estado = ModuleRequestStatus.Pendiente,
                Evidencias = dto.DocumentosEvidencia.Select(e => new UploadedDocument
                {
                    TipoDocumento = e.Tipo,
                    UrlArchivo = e.Url,
                    FechaSubida = DateTime.UtcNow
                }).ToList()
            };

            user.DatosPersonales.SolicitudesModulos.Add(newModuleRequest);
            user.TieneSolicitudPendiente = true;

            await _userRepository.UpdateAsync(user);
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            var profileDto = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                TipoUsuario = user.TipoUsuario,
                Estado = user.Estado,
                FotoPerfilUrl = user.FotoPerfilUrl,
                Biografia = user.Biografia
            };

            if (user.TipoUsuario == UserType.Personal.ToString() && user.DatosPersonales != null)
            {
                profileDto.DatosPersonales = new PersonalDataDto
                {
                    NombreCompleto = $"{user.DatosPersonales.Nombres} {user.DatosPersonales.ApellidoPaterno} {user.DatosPersonales.ApellidoMaterno}".Trim(),
                    CI = MaskDocument(user.DatosPersonales.CI, 3),
                    Celular = user.DatosPersonales.Celular,
                    Direccion = user.DatosPersonales.Direccion,
                    Tags = user.DatosPersonales.Tags.Select(t => new TagResumenDto
                    {
                        Nombre = t.Nombre,
                        Estado = t.Estado.ToString(),
                        Estrellas = t.CalificacionPromedio
                    }).ToList()
                };
            }
            else if (user.TipoUsuario == UserType.Empresa.ToString() && user.DatosEmpresa != null)
            {
                profileDto.DatosEmpresa = new CompanyDataDto
                {
                    RazonSocial = user.DatosEmpresa.RazonSocial,
                    Nit = MaskDocument(user.DatosEmpresa.Nit, 4),
                    Telefonos = $"{user.DatosEmpresa.TelefonoFijo} / {user.DatosEmpresa.CelularCorporativo}".Trim(' ', '/'),
                    Sucursales = user.DatosEmpresa.Sucursales.Select(s => new SucursalProfileDto
                    {
                        Nombre = s.NombreSucursal,
                        Direccion = s.DireccionEscrita,
                        Departamento = s.Departamento,
                        Telefono = s.TelefonoSucursal
                    }).ToList(),
                    Perfiles = user.DatosEmpresa.PerfilesComerciales.Select(p => new PerfilComercialResumenDto
                    {
                        Nombre = p.NombreComercial,
                        Tipo = p.Tipo.ToString(),
                        Estrellas = p.CalificacionPromedio
                    }).ToList()
                };
            }
            // Note: Employee/SubAccount logic can be added here as another 'else if'

            return profileDto;
        }

        public async Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            // Update common profile properties
            user.FotoPerfilUrl = dto.FotoPerfilUrl ?? user.FotoPerfilUrl;
            user.Biografia = dto.Biografia ?? user.Biografia;

            // Update personal-specific data only if the profile exists
            if (user.DatosPersonales != null)
            {
                user.DatosPersonales.Celular = dto.Celular ?? user.DatosPersonales.Celular;
                user.DatosPersonales.Direccion = dto.Direccion ?? user.DatosPersonales.Direccion;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task<PublicUserProfileDto> GetPublicProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Estado == UserStatus.Eliminado.ToString())
            {
                throw new KeyNotFoundException("Perfil no encontrado o no disponible.");
            }

            var publicProfile = new PublicUserProfileDto
            {
                Id = user.Id,
                TipoUsuario = user.TipoUsuario,
                FotoUrl = user.FotoPerfilUrl,
                Biografia = user.Biografia,
                EsVerificado = user.Estado == UserStatus.Activo.ToString()
            };

            if (user.TipoUsuario == UserType.Personal.ToString() && user.DatosPersonales != null)
            {
                var personal = user.DatosPersonales;
                publicProfile.NombreMostrar = $"{personal.Nombres} {personal.ApellidoPaterno}".Trim();
                publicProfile.Ciudad = personal.Departamento;

                if (personal.DireccionVisible)
                {
                    publicProfile.Contacto = new PublicContactInfoDto
                    {
                        Direccion = personal.Direccion,
                        Telefono = personal.Celular,
                        Ubicacion = personal.UbicacionLaboral != null ? new GeoLocationDto(Latitude: personal.UbicacionLaboral.Lat, Longitude: personal.UbicacionLaboral.Lng) : null
                    };
                }

                publicProfile.Etiquetas = personal.Tags
                    .Where(t => t.Estado == TagStatus.Activo)
                    .Select(t => new PublicTagDto
                    {
                        Nombre = t.Nombre,
                        Estrellas = t.CalificacionPromedio,
                        TotalResenas = t.TotalResenas
                    }).ToList();
            }
            else if (user.TipoUsuario == UserType.Empresa.ToString() && user.DatosEmpresa != null)
            {
                var empresa = user.DatosEmpresa;
                publicProfile.NombreMostrar = empresa.RazonSocial;
                var sucursalPrincipal = empresa.Sucursales.FirstOrDefault();
                if (sucursalPrincipal != null)
                {
                    publicProfile.Ciudad = sucursalPrincipal.Departamento;
                    publicProfile.Contacto = new PublicContactInfoDto
                    {
                        Direccion = sucursalPrincipal.DireccionEscrita,
                        Telefono = sucursalPrincipal.TelefonoSucursal ?? empresa.TelefonoFijo,
                        Ubicacion = new GeoLocationDto(Latitude: sucursalPrincipal.Latitud, Longitude: sucursalPrincipal.Longitud)
                    };
                }
                
                publicProfile.Etiquetas = empresa.PerfilesComerciales
                    .Where(p => p.Estado == CommercialProfileStatus.Activo)
                    .Select(p => new PublicTagDto 
                    {
                        Nombre = p.NombreComercial,
                        Estrellas = p.CalificacionPromedio,
                        TotalResenas = p.TotalResenas
                    }).ToList();
            }

            return publicProfile;
        }

        private string MaskDocument(string document, int visibleChars)
        {
            if (string.IsNullOrEmpty(document) || document.Length <= visibleChars)
            {
                return document;
            }
            return new string('X', document.Length - visibleChars) + document.Substring(document.Length - visibleChars);
        }
    }
}