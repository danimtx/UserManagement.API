using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Entities.ValueObjects;
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

        public async Task<List<CompanyValidationDto>> GetPendingCompaniesAsync()
        {
            var users = await _userRepository.GetPendingCompaniesAsync();

            return users.Select(u => new CompanyValidationDto
            {
                Id = u.Id,
                Email = u.Email,
                RazonSocial = u.DatosEmpresa?.RazonSocial ?? "Sin Nombre",
                Nit = u.DatosEmpresa?.Nit ?? "S/N",
                Estado = u.Estado,
                FechaRegistro = u.FechaRegistro,
                NombreRepresentante = $"{u.DatosEmpresa?.Representante?.Nombres} {u.DatosEmpresa?.Representante?.ApellidoPaterno}",
                DocumentosLegales = u.DatosEmpresa?.DocumentosLegales ?? new List<UploadedDocument>(),
                SolicitudesModulos = u.SolicitudesModulos ?? new List<ModuleRequest>()
            }).ToList();
        }

        public async Task<string> ApproveCompanyAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("La empresa no existe.");

            user.Estado = UserStatus.Activo.ToString();
            await _userRepository.UpdateAsync(user);

            return "Empresa aprobada exitosamente.";
        }

        public async Task<string> ProcessModuleRequestsAsync(ApproveModuleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.CompanyId);
            if (user == null) throw new Exception("Empresa no encontrada.");

            var modulosBase = new List<string> { "Social", "Wallet", "Market", "Trabajo", "Herramientas" };
            bool seAproboAlgunoNuevo = false;

            foreach (var decision in dto.Decisiones)
            {
                var solicitud = user.SolicitudesModulos
                    .FirstOrDefault(s => s.NombreModulo == decision.NombreModulo && s.Estado == "Pendiente");

                if (solicitud != null)
                {
                    if (decision.Aprobar)
                    {
                        solicitud.Estado = "Aprobado";

                        if (!user.ModulosHabilitados.Contains(decision.NombreModulo))
                        {
                            user.ModulosHabilitados.Add(decision.NombreModulo);
                            seAproboAlgunoNuevo = true;
                        }
                    }
                    else
                    {
                        solicitud.Estado = "Rechazado";
                        solicitud.MotivoRechazo = decision.MotivoRechazo ?? "Sin motivo especificado";
                    }
                }
            }

            if (user.ModulosHabilitados.Count > 0)
            {
                user.Estado = UserStatus.Activo.ToString();
                foreach (var basico in modulosBase)
                {
                    if (!user.ModulosHabilitados.Contains(basico))
                    {
                        user.ModulosHabilitados.Add(basico);
                    }
                }
            }

            await _userRepository.UpdateAsync(user);
            return "Solicitudes procesadas correctamente.";
        }

        public async Task<List<MarketValidationDto>> GetPendingMarketUsersAsync()
        {
            var users = await _userRepository.GetActivePersonalUsersAsync();
            var pending = new List<MarketValidationDto>();

            foreach (var user in users)
            {
                if (user.DatosPersonales == null) continue;

                if (!user.DatosPersonales.VerificadoMarket &&
                    user.DatosPersonales.DocumentosSoporte.Count > 0)
                {
                    pending.Add(new MarketValidationDto
                    {
                        Id = user.Id,
                        Nombres = user.DatosPersonales.Nombres,
                        Apellidos = $"{user.DatosPersonales.ApellidoPaterno} {user.DatosPersonales.ApellidoMaterno}",
                        CI = user.DatosPersonales.CI,
                        Profesion = user.DatosPersonales.Profesion,
                        DocumentosEvidencia = user.DatosPersonales.DocumentosSoporte
                    });
                }
            }
            return pending;
        }

        public async Task<string> VerifyMarketUserAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Usuario no encontrado.");

            if (user.DatosPersonales == null) throw new Exception("El usuario no tiene perfil personal.");
            user.DatosPersonales.VerificadoMarket = true;

            await _userRepository.UpdateAsync(user);

            return "Usuario verificado para Market exitosamente.";
        }
    }
}
