using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using System.Text;

namespace UserManagement.Application.Interfaces
{
    public interface IAdminService
    {
        // Obtener lista de empresas pendientes
        Task<List<CompanyValidationDto>> GetPendingCompaniesAsync();

        // Aprobar una empresa por su ID
        Task<string> ApproveCompanyAsync(string userId);

        // Rechazar una empresa (opcional por ahora, pero bueno tenerlo en el radar)
        // Task RejectCompanyAsync(string userId, string motivo); 
    }
}