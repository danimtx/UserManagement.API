using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.DTOs.Admin.Support;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityProvider _identityProvider;

        public AdminService(IUserRepository userRepository, IIdentityProvider identityProvider)
        {
            _userRepository = userRepository;
            _identityProvider = identityProvider;
        }

        // --- Bandeja 1: Identidades ---
        public async Task<List<PendingIdentityDto>> GetPendingIdentitiesAsync()
        {
            var users = await _userRepository.GetPendingCompaniesAsync();
            return users.Select(u => new PendingIdentityDto
            {
                UserId = u.Id,
                RazonSocial = u.DatosEmpresa?.RazonSocial ?? "N/A",
                Nit = u.DatosEmpresa?.Nit ?? "N/A",
                FechaRegistro = u.FechaRegistro
            }).ToList();
        }

        public async Task DecideOnIdentityAsync(IdentityDecisionDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null || user.TipoUsuario != UserType.Empresa.ToString()) throw new Exception("Usuario de empresa no encontrado o inválido.");

            user.Estado = dto.Approve ? UserStatus.Activo.ToString() : UserStatus.Rechazado.ToString();
            
            await _userRepository.UpdateAsync(user);
        }

        // --- Bandeja 2: Módulos Técnicos ---
        public async Task<List<PendingModuleDto>> GetPendingModulesAsync()
        {
            var companies = await _userRepository.GetCompaniesWithPendingModulesAsync();
            var pendingModules = new List<PendingModuleDto>();

            foreach (var company in companies)
            {
                if (company.DatosEmpresa == null) continue;

                var profiles = company.DatosEmpresa.PerfilesComerciales
                    .Where(p => p.Tipo == CommercialProfileType.Modulo && p.Estado == CommercialProfileStatus.Pendiente);

                foreach (var profile in profiles)
                {
                    pendingModules.Add(new PendingModuleDto
                    {
                        CompanyUserId = company.Id,
                        CompanyName = company.DatosEmpresa.RazonSocial,
                        CommercialProfileId = profile.Id,
                        ProfileName = profile.NombreComercial,
                        ModuleName = profile.ModuloAsociado ?? "N/A",
                        RequestDate = profile.DocumentosEspecificos.FirstOrDefault()?.FechaSubida ?? DateTime.MinValue
                    });
                }
            }
            return pendingModules;
        }

        public async Task DecideOnModuleAsync(ModuleDecisionDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.CompanyUserId);
            if (user == null || user.DatosEmpresa == null) throw new Exception("Usuario de empresa no encontrado.");

            var profile = user.DatosEmpresa.PerfilesComerciales.FirstOrDefault(p => p.Id == dto.CommercialProfileId);
            if (profile == null) throw new Exception("Perfil comercial no encontrado.");

            profile.Estado = dto.Approve ? CommercialProfileStatus.Activo : CommercialProfileStatus.Rechazado;

            if (dto.Approve && !string.IsNullOrEmpty(profile.ModuloAsociado))
            {
                if (!user.ModulosHabilitados.Contains(profile.ModuloAsociado))
                {
                    user.ModulosHabilitados.Add(profile.ModuloAsociado);
                }
            }

            await _userRepository.UpdateAsync(user);
        }

        // --- Bandeja 3: Tags y Reputación ---
        public async Task<List<PendingTagDto>> GetPendingTagsAsync()
        {
            var users = await _userRepository.GetUsersWithPendingTagsAsync();
            var pendingTags = new List<PendingTagDto>();

            foreach (var user in users)
            {
                if (user.TipoUsuario == UserType.Empresa.ToString() && user.DatosEmpresa != null)
                {
                    var profiles = user.DatosEmpresa.PerfilesComerciales
                        .Where(p => p.Tipo == CommercialProfileType.TagSocial && p.Estado == CommercialProfileStatus.Pendiente);
                    
                    foreach(var profile in profiles)
                    {
                         pendingTags.Add(new PendingTagDto { /* ... mapping ... */ });
                    }
                }
                else if (user.TipoUsuario == UserType.Personal.ToString() && user.DatosPersonales != null)
                {
                    var tags = user.DatosPersonales.Tags.Where(t => t.Estado == TagStatus.Pendiente);

                    foreach(var tag in tags)
                    {
                        pendingTags.Add(new PendingTagDto { /* ... mapping ... */ });
                    }
                }
            }
            return pendingTags;
        }

        public async Task DecideOnCompanyTagAsync(CompanyTagDecisionDto dto)
        {
             var user = await _userRepository.GetByIdAsync(dto.CompanyUserId);
            if (user == null || user.DatosEmpresa == null) throw new Exception("Usuario de empresa no encontrado.");

            var profile = user.DatosEmpresa.PerfilesComerciales.FirstOrDefault(p => p.Id == dto.CommercialProfileId);
            if (profile == null) throw new Exception("Perfil comercial no encontrado.");

            profile.Estado = dto.Approve ? CommercialProfileStatus.Activo : CommercialProfileStatus.Rechazado;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DecideOnPersonalTagAsync(PersonalTagDecisionDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.PersonalUserId);
            if (user == null || user.DatosPersonales == null) throw new Exception("Usuario personal no encontrado.");

            var tag = user.DatosPersonales.Tags.FirstOrDefault(t => t.Nombre.Equals(dto.TagName, StringComparison.OrdinalIgnoreCase));
            if (tag == null) throw new Exception("Tag no encontrado en el perfil.");

            tag.Estado = dto.Approve ? TagStatus.Activo : TagStatus.Rechazado;
            await _userRepository.UpdateAsync(user);
        }

        // --- Bandeja 4: Módulos Personales ---
        public async Task<List<PendingModuleDto>> GetPendingPersonalModulesAsync()
        {
            var users = await _userRepository.GetUsersWithPendingModuleRequestsAsync();
            // ... implementation ...
            return new List<PendingModuleDto>();
        }

        public async Task DecideOnPersonalModuleAsync(PersonalModuleDecisionDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.PersonalUserId);
            if (user == null || user.DatosPersonales == null) throw new Exception("Usuario personal no encontrado.");
            // ... implementation ...
            await _userRepository.UpdateAsync(user);
        }

        // --- Support & Moderation Module ---

        public async Task<List<AdminUserSearchResultDto>> SearchUsersAsync(string term)
        {
            var results = new List<User>();
            User? user = null;

            user = await _userRepository.GetByEmailAsync(term);
            if (user != null) results.Add(user);

            if (results.Count == 0)
            {
                user = await _userRepository.GetByUserNameAsync(term);
                if (user != null) results.Add(user);
            }

            if (results.Count == 0)
            {
                user = await _userRepository.GetByCiAsync(term);
                if (user != null) results.Add(user);
            }
            
            if (results.Count == 0)
            {
                user = await _userRepository.GetByNitAsync(term);
                if (user != null) results.Add(user);
            }

            return results.Select(u => new AdminUserSearchResultDto
            {
                Id = u.Id,
                IdentificadorPrincipal = u.Email,
                NombreDisplay = u.TipoUsuario == UserType.Personal.ToString() ? $"{u.DatosPersonales?.Nombres} {u.DatosPersonales?.ApellidoPaterno}".Trim() : u.DatosEmpresa?.RazonSocial ?? string.Empty,
                TipoUsuario = u.TipoUsuario,
                Estado = u.Estado
            }).ToList();
        }

        public async Task<AdminUserDetailDto> GetUserDetailAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("Usuario no encontrado.");

            var detailDto = new AdminUserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                TipoUsuario = user.TipoUsuario,
                Estado = user.Estado,
                FechaRegistro = user.FechaRegistro
            };

            if (user.DatosPersonales != null)
            {
                detailDto.DatosPersonales = new AdminPersonalProfileDto
                {
                    Nombres = user.DatosPersonales.Nombres,
                    ApellidoPaterno = user.DatosPersonales.ApellidoPaterno,
                    ApellidoMaterno = user.DatosPersonales.ApellidoMaterno,
                    FechaNacimiento = user.DatosPersonales.FechaNacimiento,
                    CI = user.DatosPersonales.CI,
                    Nit = user.DatosPersonales.Nit,
                    Direccion = user.DatosPersonales.Direccion,
                    Celular = user.DatosPersonales.Celular,
                    Profesion = user.DatosPersonales.Profesion
                };
            }

            if (user.DatosEmpresa != null)
            {
                detailDto.DatosEmpresa = new AdminCompanyProfileDto
                {
                    RazonSocial = user.DatosEmpresa.RazonSocial,
                    Nit = user.DatosEmpresa.Nit
                };
            }

            return detailDto;
        }

        public async Task ChangeUserStatusAsync(string userId, AdminChangeStatusDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("Usuario no encontrado.");

            if (!Enum.TryParse<UserStatus>(dto.NuevoEstado, true, out var newStatus))
            {
                throw new ArgumentException($"El estado '{dto.NuevoEstado}' no es un estado válido.");
            }

            user.Estado = newStatus.ToString();
            
            await _userRepository.UpdateAsync(user);
        }

        public async Task AdminResetUserPasswordAsync(string userId, string newPassword)
        {
            await _identityProvider.AdminResetPasswordAsync(userId, newPassword);
        }
    }
}
