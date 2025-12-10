using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IIdentityProvider identityProvider, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _identityProvider = identityProvider;
            _configuration = configuration;
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
                string fotoPerfilEncontrada = string.Empty;

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

                        if (d.Tipo.Contains("Perfil", StringComparison.OrdinalIgnoreCase) ||
                            d.Tipo.Contains("Avatar", StringComparison.OrdinalIgnoreCase))
                        {
                            fotoPerfilEncontrada = d.Url;
                        }
                    }
                }

                var newUser = new User
                {
                    Id = createdUid,
                    Email = dto.Email,

                    UserName = dto.UserName,
                    FotoPerfilUrl = fotoPerfilEncontrada,

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
                        Nit = dto.Nit,
                        CodigoSeprec = dto.Seprec,

                        Pais = dto.Pais,
                        Departamento = dto.Departamento,
                        Direccion = dto.Direccion,
                        Celular = dto.Celular,

                        Profesion = dto.Profesion,
                        LinkedInUrl = dto.LinkedInUrl,


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
                string logoEncontrado = string.Empty;

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

                        if (doc.Tipo.Contains("Logo", StringComparison.OrdinalIgnoreCase))
                        {
                            logoEncontrado = doc.Url;
                        }
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

                var solicitudes = new List<ModuleRequest>();
                if (dto.ModulosSolicitados != null)
                {
                    foreach (var mod in dto.ModulosSolicitados)
                    {
                        solicitudes.Add(new ModuleRequest
                        {
                            NombreModulo = mod,
                            Estado = "Pendiente",
                            FechaSolicitud = DateTime.UtcNow
                        });
                    }
                }

                var newUser = new User
                {
                    Id = createdUid,
                    Email = dto.EmailEmpresa,
                    UserName = null,

                    FotoPerfilUrl = logoEncontrado,

                    TipoUsuario = UserType.Empresa.ToString(),
                    Estado = UserStatus.Pendiente.ToString(),
                    ModulosHabilitados = new List<string>(),
                    SolicitudesModulos = solicitudes,
                    FechaRegistro = DateTime.UtcNow,

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

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var (firebaseToken, uid, refreshToken) = await _identityProvider.SignInAsync(loginDto.Email, loginDto.Password);
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
            
            var customToken = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = customToken,
                RefreshToken = refreshToken,
                UserId = uid
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string incomingRefreshToken)
        {
            var (newFirebaseToken, newRefreshToken) = await _identityProvider.RefreshTokenAsync(incomingRefreshToken);
            
            var userId = DecodeFirebaseToken(newFirebaseToken);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("El usuario asociado al token de refresco ya no existe.");
            }

            var newCustomToken = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = newCustomToken,
                RefreshToken = newRefreshToken,
                UserId = userId
            };
        }

        private string DecodeFirebaseToken(string firebaseToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(firebaseToken);
            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "user_id");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("El token de Firebase no contiene el user_id.");
            }
            return userIdClaim.Value;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.TipoUsuario)
            };

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(60); // Access token expires in 60 minutes

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
