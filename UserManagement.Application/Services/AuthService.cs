using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityProvider _identityProvider;

        public AuthService(IUserRepository userRepository, IIdentityProvider identityProvider)
        {
            _userRepository = userRepository;
            _identityProvider = identityProvider;
        }

        public async Task<string> RegisterPersonalAsync(RegisterPersonalDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ArgumentException("Las contraseñas ingresadas no coinciden.");

            var usuarioExistente = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (usuarioExistente != null)
            {
                throw new ArgumentException($"El nombre de usuario '{dto.UserName}' ya está ocupado.");
            }

            string createdUid = "";

            try
            {
                createdUid = await _identityProvider.CreateUserAsync(
                    dto.Email,
                    dto.Password,
                    $"{dto.Nombres} {dto.ApellidoPaterno}"
                );

                var docsSoporte = new List<UploadedDocument>();
                string fotoCiUrlEncontrada = string.Empty;
                string fotoTituloUrlEncontrada = null;

                if (dto.Documentos != null)
                {
                    foreach (var d in dto.Documentos)
                    {
                        var doc = new UploadedDocument
                        {
                            TipoDocumento = d.Tipo,
                            UrlArchivo = d.Url,
                            ModuloObjetivo = d.Modulo,
                            EstadoValidacion = "Pendiente",
                            FechaSubida = DateTime.UtcNow
                        };
                        docsSoporte.Add(doc);

                        if (d.Tipo.Contains("CI", StringComparison.OrdinalIgnoreCase))
                            fotoCiUrlEncontrada = d.Url;

                        if (d.Tipo.Contains("Titulo", StringComparison.OrdinalIgnoreCase))
                            fotoTituloUrlEncontrada = d.Url;
                    }
                }

                var newUser = new User
                {
                    Id = createdUid,
                    Email = dto.Email,
                    UserName = dto.UserName,
                    TipoUsuario = UserType.Personal.ToString(),
                    Estado = UserStatus.Activo.ToString(),
                    FechaRegistro = DateTime.UtcNow,
                    ModulosHabilitados = new List<string> { "Social", "Wallet", "Market" },

                    DatosPersonales = new PersonalProfile
                    {
                        Nombres = dto.Nombres,
                        ApellidoPaterno = dto.ApellidoPaterno,
                        ApellidoMaterno = dto.ApellidoMaterno ?? string.Empty,
                        FechaNacimiento = dto.FechaNacimiento.ToUniversalTime(),
                        CI = dto.CI,
                        Pais = dto.Pais,
                        Departamento = dto.Departamento,
                        Direccion = dto.Direccion,
                        Celular = dto.Celular,
                        Profesion = dto.Profesion,
                        LinkedInUrl = dto.LinkedInUrl,
                        Nit = dto.Nit,

                        FotoCiUrl = fotoCiUrlEncontrada,
                        FotoTituloUrl = fotoTituloUrlEncontrada,

                        VerificadoMarket = false,
                        DocumentosSoporte = docsSoporte
                    },

                    IdsPerfilesSociales = new List<string>()
                };

                await _userRepository.AddAsync(newUser);
                return createdUid;
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(createdUid)) await _identityProvider.DeleteUserAsync(createdUid);
                throw;
            }
        }

        public async Task<string> RegisterCompanyAsync(RegisterCompanyDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ArgumentException("Las contraseñas no coinciden.");

            string createdUid = "";

            try
            {
                createdUid = await _identityProvider.CreateUserAsync(dto.EmailEmpresa, dto.Password, dto.RazonSocial);

                var docsLegales = new List<UploadedDocument>();
                if (dto.DocumentosLegales != null)
                {
                    foreach (var doc in dto.DocumentosLegales)
                    {
                        docsLegales.Add(new UploadedDocument
                        {
                            TipoDocumento = doc.Tipo,
                            UrlArchivo = doc.Url,
                            ModuloObjetivo = doc.Modulo,
                            EstadoValidacion = "Pendiente",
                            FechaSubida = DateTime.UtcNow
                        });
                    }
                }

                var sucursales = new List<CompanyBranch>();
                if (dto.Sucursales != null && dto.Sucursales.Count > 0)
                {
                    foreach (var suc in dto.Sucursales)
                    {
                        sucursales.Add(new CompanyBranch
                        {
                            NombreSucursal = suc.Nombre,
                            DireccionEscrita = suc.Direccion,
                            Departamento = suc.Departamento,
                            Pais = suc.Pais,
                            Latitud = suc.Latitud,
                            Longitud = suc.Longitud,
                            TelefonoSucursal = suc.Telefono,
                            ModulosAsociados = suc.ModulosAsociados,
                            EsActiva = true
                        });
                    }
                }
                else
                {
                    sucursales.Add(new CompanyBranch
                    {
                        NombreSucursal = "Oficina Central",
                        DireccionEscrita = "Sin dirección",
                        Pais = "Bolivia",
                        EsActiva = true
                    });
                }

                var newUser = new User
                {
                    Id = createdUid,
                    Email = dto.EmailEmpresa,
                    TipoUsuario = UserType.Empresa.ToString(),
                    Estado = UserStatus.Pendiente.ToString(),
                    FechaRegistro = DateTime.UtcNow,
                    ModulosHabilitados = dto.ModulosSolicitados,

                    DatosEmpresa = new CompanyProfile
                    {
                        RazonSocial = dto.RazonSocial,
                        TipoEmpresa = dto.TipoEmpresa,
                        Nit = dto.Nit,

                        TelefonoFijo = dto.TelefonoFijo,
                        CelularCorporativo = dto.CelularCorporativo,

                        Representante = new LegalRepresentative
                        {
                            Nombres = dto.Representante.Nombres,
                            ApellidoPaterno = dto.Representante.ApellidoPaterno,
                            ApellidoMaterno = dto.Representante.ApellidoMaterno ?? string.Empty,
                            CI = dto.Representante.CI,
                            FechaNacimiento = dto.Representante.FechaNacimiento.ToUniversalTime(),
                            DireccionDomicilio = dto.Representante.DireccionDomicilio,
                            EmailPersonal = dto.Representante.EmailPersonal,
                            NumerosContacto = dto.Representante.NumerosContacto
                        },

                        DocumentosLegales = docsLegales,
                        Sucursales = sucursales,
                        AreasDefinidas = new List<string> { "General", "Administración" }
                    },

                    IdsPerfilesSociales = new List<string>()
                };

                await _userRepository.AddAsync(newUser);
                return createdUid;
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(createdUid)) await _identityProvider.DeleteUserAsync(createdUid);
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var (token, uid) = await _identityProvider.SignInAsync(loginDto.Email, loginDto.Password);
            var user = await _userRepository.GetByIdAsync(uid);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Credenciales válidas, pero el usuario no tiene perfil en el sistema.");
            }

            if (user.TipoUsuario == UserType.Empresa.ToString() && user.Estado == UserStatus.Pendiente.ToString())
            {
                throw new UnauthorizedAccessException("Su cuenta de empresa está en revisión. Espere la aprobación del Administrador.");
            }

            if (user.Estado == UserStatus.Suspendido.ToString() || user.Estado == UserStatus.Rechazado.ToString())
            {
                throw new UnauthorizedAccessException($"Su cuenta está {user.Estado}. Contacte a soporte.");
            }

            if (!string.IsNullOrEmpty(loginDto.ModuloSeleccionado))
            {
                bool esSuperAdmin = user.TipoUsuario == UserType.SuperAdminGlobal.ToString() ||
                                    user.TipoUsuario == UserType.AdminSistema.ToString();

                if (!esSuperAdmin && !user.ModulosHabilitados.Contains(loginDto.ModuloSeleccionado))
                {
                    throw new UnauthorizedAccessException($"No tienes permiso para acceder al módulo: {loginDto.ModuloSeleccionado}.");
                }
            }

            return token;
        }
    }
}
