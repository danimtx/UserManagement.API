using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;

namespace UserManagement.Application.Interfaces.Services
{
    public interface IAdminService
    {
        // --- Bandeja 1: Identidades ---
        Task<List<PendingIdentityDto>> GetPendingIdentitiesAsync();
        Task DecideOnIdentityAsync(IdentityDecisionDto dto);

        // --- Bandeja 2: Módulos Técnicos ---
        Task<List<PendingModuleDto>> GetPendingModulesAsync();
        Task DecideOnModuleAsync(ModuleDecisionDto dto);

        // --- Bandeja 3: Tags y Reputación ---
        Task<List<PendingTagDto>> GetPendingTagsAsync();
        Task DecideOnCompanyTagAsync(CompanyTagDecisionDto dto);
        Task DecideOnPersonalTagAsync(PersonalTagDecisionDto dto);

        // --- Bandeja 4: Módulos Personales ---
        Task<List<PendingModuleDto>> GetPendingPersonalModulesAsync();
        Task DecideOnPersonalModuleAsync(PersonalModuleDecisionDto dto);
    }
}