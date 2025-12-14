using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            // En un caso real, aquí se podría guardar el dto.RejectionReason en algún log o campo.
            
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
            // Aquí también se podría guardar el dto.RejectionReason en el perfil.

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
                // Solicitudes de Tags de Empresas
                if (user.TipoUsuario == UserType.Empresa.ToString() && user.DatosEmpresa != null)
                {
                    var profiles = user.DatosEmpresa.PerfilesComerciales
                        .Where(p => p.Tipo == CommercialProfileType.TagSocial && p.Estado == CommercialProfileStatus.Pendiente);
                    
                    foreach(var profile in profiles)
                    {
                         pendingTags.Add(new PendingTagDto
                         {
                            RequestType = "Empresa",
                            UserId = user.Id,
                            UserName = user.DatosEmpresa.RazonSocial,
                            TagOrProfileId = profile.Id,
                            TagOrProfileName = profile.NombreComercial,
                            RequestDate = profile.DocumentosEspecificos.FirstOrDefault()?.FechaSubida ?? DateTime.MinValue
                         });
                    }
                }
                // Solicitudes de Tags de Personas
                else if (user.TipoUsuario == UserType.Personal.ToString() && user.DatosPersonales != null)
                {
                    var tags = user.DatosPersonales.Tags.Where(t => t.Estado == TagStatus.Pendiente);

                    foreach(var tag in tags)
                    {
                        pendingTags.Add(new PendingTagDto
                        {
                            RequestType = "Personal",
                            UserId = user.Id,
                            UserName = $"{user.DatosPersonales.Nombres} {user.DatosPersonales.ApellidoPaterno}",
                            TagOrProfileId = tag.Nombre, // Para persona, el ID es el nombre del tag
                            TagOrProfileName = tag.Nombre,
                            RequestDate = tag.Evidencias.FirstOrDefault()?.FechaSubida ?? DateTime.MinValue
                        });
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
            var pendingModules = new List<PendingModuleDto>();

            foreach (var user in users)
            {
                if (user.DatosPersonales == null) continue;

                var moduleRequests = user.DatosPersonales.SolicitudesModulos
                    .Where(m => m.Estado == ModuleRequestStatus.Pendiente);

                foreach (var request in moduleRequests)
                {
                    pendingModules.Add(new PendingModuleDto
                    {
                        CompanyUserId = user.Id, // Re-using this field for PersonalUserId
                        CompanyName = $"{user.DatosPersonales.Nombres} {user.DatosPersonales.ApellidoPaterno}", // Re-using for personal name
                        CommercialProfileId = request.NombreModulo, // Re-using for module name
                        ProfileName = request.NombreModulo,
                        ModuleName = request.NombreModulo,
                        RequestDate = request.Evidencias.FirstOrDefault()?.FechaSubida ?? DateTime.MinValue
                    });
                }
            }
            return pendingModules;
        }

        public async Task DecideOnPersonalModuleAsync(PersonalModuleDecisionDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.PersonalUserId);
            if (user == null || user.DatosPersonales == null) throw new Exception("Usuario personal no encontrado.");

            var moduleRequest = user.DatosPersonales.SolicitudesModulos.FirstOrDefault(m => m.NombreModulo.Equals(dto.ModuleName, StringComparison.OrdinalIgnoreCase));
            if (moduleRequest == null) throw new Exception("Solicitud de módulo no encontrada.");

            moduleRequest.Estado = dto.Approve ? ModuleRequestStatus.Aprobado : ModuleRequestStatus.Rechazado;
            moduleRequest.MotivoRechazo = dto.Approve ? null : dto.RejectionReason;

            if (dto.Approve)
            {
                if (!user.ModulosHabilitados.Contains(dto.ModuleName))
                {
                    user.ModulosHabilitados.Add(dto.ModuleName);
                }
            }

            // Recalcular si quedan solicitudes pendientes
            bool hasPending = user.DatosPersonales.Tags.Any(t => t.Estado == TagStatus.Pendiente) ||
                              user.DatosPersonales.SolicitudesModulos.Any(m => m.Estado == ModuleRequestStatus.Pendiente);
            
            if (user.DatosEmpresa != null)
            {
                hasPending = hasPending || user.DatosEmpresa.PerfilesComerciales.Any(p => p.Estado == CommercialProfileStatus.Pendiente);
            }

            user.TieneSolicitudPendiente = hasPending;

            await _userRepository.UpdateAsync(user);
        }
    }
}
