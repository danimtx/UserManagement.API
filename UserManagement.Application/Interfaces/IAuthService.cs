using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Auth; // Importante para encontrar los nuevos DTOs

namespace UserManagement.Application.Interfaces
{
    public interface IAuthService
    {
        // Registro para personas (crea usuario Activo)
        Task<string> RegisterPersonalAsync(RegisterPersonalDto dto);

        // Registro para empresas (crea usuario Pendiente)
        Task<string> RegisterCompanyAsync(RegisterCompanyDto dto);

        // Login unificado (sirve para todos)
        Task<string> LoginAsync(LoginDto loginDto);
    }
}