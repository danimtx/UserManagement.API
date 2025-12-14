using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.DTOs.Company.Employees;
using UserManagement.Application.DTOs.Public;
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
                if (empresa.DatosEmpresa.AreasDefinidas == null)
                {
                    empresa.DatosEmpresa.AreasDefinidas = new List<string>();
                }
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

            user.DatosEmpresa.RazonSocial = dto.RazonSocial;
            user.DatosEmpresa.Nit = dto.Nit;

            if (dto.DocumentosLegales.Any())
            {
                user.DatosEmpresa.DocumentosLegales = dto.DocumentosLegales.Select(d => new UploadedDocument
                {
                    TipoDocumento = d.Tipo,
                    UrlArchivo = d.Url,
                    FechaSubida = DateTime.UtcNow
                }).ToList();
            }
            
            user.Estado = UserStatus.Pendiente.ToString();

            await _userRepository.UpdateAsync(user);
        }

        private async Task<User> ValidateEmployeeOwnership(string companyId, string employeeId)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException("Empleado no encontrado.");
            }
            if (employee.CreadoPorId != companyId)
            {
                throw new UnauthorizedAccessException("Este empleado no pertenece a su empresa.");
            }
            return employee;
        }

        public async Task<List<EmployeeSummaryDto>> GetEmployeesAsync(string companyId)
        {
            var employees = await _userRepository.GetEmployeesByCompanyAsync(companyId);
            return employees.Select(e => new EmployeeSummaryDto
            {
                Id = e.Id,
                NombreCompleto = $"{e.DatosPersonales?.Nombres} {e.DatosPersonales?.ApellidoPaterno}".Trim(),
                Email = e.Email,
                Cargo = e.AreaTrabajo,
                EsSuperAdmin = e.EsSuperAdminEmpresa,
                Estado = e.Estado,
                FechaRegistro = e.FechaRegistro
            }).ToList();
        }

        public async Task<EmployeeDetailDto> GetEmployeeDetailAsync(string companyId, string employeeId)
        {
            var employee = await ValidateEmployeeOwnership(companyId, employeeId);
            
            var permissions = new Dictionary<string, ModuleAccessDto>();
            if (employee.PermisosEmpleado?.Modulos != null)
            {
                permissions = employee.PermisosEmpleado.Modulos.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new ModuleAccessDto
                    {
                        Acceso = kvp.Value.TieneAcceso,
                        Funcionalidades = kvp.Value.FuncionalidadesPermitidas,
                        RecursosIds = kvp.Value.RecursosEspecificosAllowed
                    });
            }

            return new EmployeeDetailDto
            {
                Id = employee.Id,
                NombreCompleto = $"{employee.DatosPersonales?.Nombres} {employee.DatosPersonales?.ApellidoPaterno}".Trim(),
                Email = employee.Email,
                Cargo = employee.AreaTrabajo,
                EsSuperAdmin = employee.EsSuperAdminEmpresa,
                Estado = employee.Estado,
                FechaRegistro = employee.FechaRegistro,
                CI = employee.DatosPersonales?.CI,
                Celular = employee.DatosPersonales?.Celular,
                Direccion = employee.DatosPersonales?.Direccion,
                Permisos = permissions
            };
        }

        public async Task UpdateEmployeePermissionsAsync(string companyId, string employeeId, UpdateEmployeePermissionsDto dto)
        {
            var employee = await ValidateEmployeeOwnership(companyId, employeeId);

            if (dto.AreaTrabajo != null)
            {
                employee.AreaTrabajo = dto.AreaTrabajo;
            }
            if (dto.EsSuperAdmin.HasValue)
            {
                employee.EsSuperAdminEmpresa = dto.EsSuperAdmin.Value;
            }
            if (dto.Permisos != null)
            {
                if(employee.PermisosEmpleado == null) employee.PermisosEmpleado = new SubAccountPermissions();
                
                employee.PermisosEmpleado.Modulos = dto.Permisos.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new ModuleAccess
                    {
                        TieneAcceso = kvp.Value.Acceso,
                        FuncionalidadesPermitidas = kvp.Value.Funcionalidades,
                        RecursosEspecificosAllowed = kvp.Value.RecursosIds
                    });
                
                employee.ModulosHabilitados = dto.Permisos
                    .Where(p => p.Value.Acceso)
                    .Select(p => p.Key)
                    .ToList();
            }

            await _userRepository.UpdateAsync(employee);
        }

        public async Task UpdateEmployeeStatusAsync(string companyId, string employeeId, UserStatus nuevoEstado)
        {
            var employee = await ValidateEmployeeOwnership(companyId, employeeId);
            employee.Estado = nuevoEstado.ToString();
            await _userRepository.UpdateAsync(employee);
        }

        public async Task ResetEmployeePasswordAsync(string companyId, string employeeId, string newPassword)
        {
            var employee = await ValidateEmployeeOwnership(companyId, employeeId);
            await _identityProvider.AdminResetPasswordAsync(employee.Id, newPassword);
        }

        public async Task UpdateEmployeeProfileAsync(string companyId, string employeeId, UpdateEmployeeProfileDto dto)
        {
            var employee = await ValidateEmployeeOwnership(companyId, employeeId);
            if(employee.DatosPersonales == null)
            {
                employee.DatosPersonales = new PersonalProfile();
            }

            if(dto.Nombres != null) employee.DatosPersonales.Nombres = dto.Nombres;
            if(dto.ApellidoPaterno != null) employee.DatosPersonales.ApellidoPaterno = dto.ApellidoPaterno;
            if(dto.ApellidoMaterno != null) employee.DatosPersonales.ApellidoMaterno = dto.ApellidoMaterno;
            if(dto.CI != null) employee.DatosPersonales.CI = dto.CI;
            if(dto.Celular != null) employee.DatosPersonales.Celular = dto.Celular;
            if(dto.Direccion != null) employee.DatosPersonales.Direccion = dto.Direccion;

            await _userRepository.UpdateAsync(employee);
        }

        public async Task<List<PublicCommercialProfileDto>> GetCompanyPublicProfilesAsync(string companyId)
        {
            var company = await _userRepository.GetByIdAsync(companyId);
            if (company == null || company.TipoUsuario != UserType.Empresa.ToString() || company.DatosEmpresa == null)
            {
                throw new KeyNotFoundException("Empresa no encontrada o no válida.");
            }

            return company.DatosEmpresa.PerfilesComerciales
                .Where(p => p.Estado == CommercialProfileStatus.Activo)
                .Select(p => new PublicCommercialProfileDto
                {
                    Id = p.Id,
                    Nombre = p.NombreComercial,
                    Tipo = p.Tipo.ToString(),
                    LogoUrl = p.LogoUrl,
                    RubroModulo = p.ModuloAsociado,
                    Estrellas = p.CalificacionPromedio,
                    TotalResenas = p.TotalResenas
                })
                .ToList();
        }
    }
}