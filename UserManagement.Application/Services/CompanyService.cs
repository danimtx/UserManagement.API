using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityProvider _identityProvider;

        public CompanyService(IUserRepository userRepository, IIdentityProvider identityProvider)
        {
            _userRepository = userRepository;
            _identityProvider = identityProvider;
        }

        public async Task<string> CreateEmployeeAsync(string companyId, CreateEmployeeDto dto)
        {
            var empresa = await _userRepository.GetByIdAsync(companyId);
            if (empresa == null) throw new Exception("La empresa no existe.");
            if (empresa.TipoUsuario != UserType.Empresa.ToString())
                throw new Exception("Solo las cuentas de Empresa pueden crear empleados.");

            string createdUid = "";

            try
            {
                createdUid = await _identityProvider.CreateUserAsync(
                    dto.Email,
                    dto.PasswordTemporal,
                    $"{dto.Nombres} {dto.ApellidoPaterno}"
                );

                var permisosMapeados = new Dictionary<string, ModuleAccess>();
                foreach (var item in dto.Permisos)
                {
                    if (item.Key.ToLower() == "wallet" || item.Key.ToLower() == "market") continue;
                    if (!empresa.ModulosHabilitados.Contains(item.Key)) continue;

                    permisosMapeados.Add(item.Key, new ModuleAccess
                    {
                        TieneAcceso = item.Value.Acceso,
                        FuncionalidadesPermitidas = item.Value.Funcionalidades,
                        RecursosEspecificosAllowed = item.Value.RecursosIds
                    });
                }

                var newEmployee = new User
                {
                    Id = createdUid,
                    Email = dto.Email,
                    TipoUsuario = UserType.SubCuentaEmpresa.ToString(),
                    Estado = UserStatus.Activo.ToString(),
                    FechaRegistro = DateTime.UtcNow,
                    CreadoPorId = companyId,
                    AreaTrabajo = dto.AreaTrabajo,
                    EsSuperAdminEmpresa = dto.EsSuperAdmin,

                    DatosPersonales = new PersonalProfile
                    {
                        Nombres = dto.Nombres,
                        ApellidoPaterno = dto.ApellidoPaterno,
                        ApellidoMaterno = dto.ApellidoMaterno,
                        CI = dto.CI,
                        Direccion = dto.DireccionEscrita,
                        Celular = dto.Celular,
                        Pais = "Bolivia",
                        FechaNacimiento = dto.FechaNacimiento
                    },

                    PermisosEmpleado = new SubAccountPermissions { Modulos = permisosMapeados },
                    ModulosHabilitados = permisosMapeados.Keys.ToList()
                };

                await _userRepository.AddAsync(newEmployee);

                return createdUid;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(createdUid))
                {
                    await _identityProvider.DeleteUserAsync(createdUid);
                }
                throw new Exception($"Error al crear empleado: {ex.Message}");
            }
        }

        public async Task AddCompanyAreaAsync(string companyId, string newAreaName)
        {
            var empresa = await _userRepository.GetByIdAsync(companyId);
            if (empresa == null) throw new Exception("Empresa no encontrada");

            if (empresa.DatosEmpresa != null)
            {
                empresa.DatosEmpresa.AreasDefinidas.Add(newAreaName);
                await _userRepository.UpdateAsync(empresa);
            }
        }

        public async Task RequestCommercialProfileAsync(string companyId, RequestCommercialProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(companyId);
            if (user == null) throw new Exception("Usuario de empresa no encontrado.");
            if (user.TipoUsuario != UserType.Empresa.ToString() || user.DatosEmpresa == null) throw new Exception("Esta acción solo es válida para perfiles de empresa.");

            var newProfile = new PerfilComercial
            {
                NombreComercial = dto.NombreComercial,
                ModuloAsociado = dto.ModuloAsociado,
                LogoUrl = dto.LogoUrl,
                Tipo = string.IsNullOrEmpty(dto.ModuloAsociado) ? CommercialProfileType.TagSocial : CommercialProfileType.Modulo,
                Estado = CommercialProfileStatus.Pendiente,
                DocumentosEspecificos = dto.Documentos.Select(d => new UploadedDocument
                {
                    TipoDocumento = d.Tipo,
                    UrlArchivo = d.Url,
                    FechaSubida = DateTime.UtcNow
                }).ToList()
            };
            
            user.DatosEmpresa.PerfilesComerciales.Add(newProfile);
            await _userRepository.UpdateAsync(user);
        }

        public async Task RectifyIdentityAsync(string companyId, RectifyIdentityDto dto)
        {
            var user = await _userRepository.GetByIdAsync(companyId);
            if (user == null) throw new Exception("Usuario de empresa no encontrado.");
            if (user.TipoUsuario != UserType.Empresa.ToString() || user.DatosEmpresa == null) throw new Exception("Esta acción solo es válida para perfiles de empresa.");

            if (user.Estado != UserStatus.Rechazado.ToString())
            {
                throw new Exception("Solo se puede rectificar una identidad que ha sido rechazada.");
            }

            // Actualizar datos
            user.DatosEmpresa.RazonSocial = dto.RazonSocial;
            user.DatosEmpresa.Nit = dto.Nit;

            // Si se envían nuevos documentos, reemplazarlos
            if (dto.DocumentosLegales.Any())
            {
                user.DatosEmpresa.DocumentosLegales = dto.DocumentosLegales.Select(d => new UploadedDocument
                {
                    TipoDocumento = d.Tipo,
                    UrlArchivo = d.Url,
                    FechaSubida = DateTime.UtcNow
                }).ToList();
            }
            
            // Cambiar estado para que vuelva a la bandeja de pendientes del admin
            user.Estado = UserStatus.Pendiente.ToString();

            await _userRepository.UpdateAsync(user);
        }
    }
}
