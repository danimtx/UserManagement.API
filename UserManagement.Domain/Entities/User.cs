using UserManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.Entities.ValueObjects;

namespace UserManagement.Domain.Entities
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }

        // Jerarquía
        public string? CreadoPorId { get; set; }

        // --- PERFILES ---
        public PersonalProfile? DatosPersonales { get; set; }
        public CompanyProfile? DatosEmpresa { get; set; }

        // --- SUB-CUENTAS ---
        public string? CuentaPadreId { get; set; }

        public string? AreaTrabajo { get; set; }
        public bool EsSuperAdminEmpresa { get; set; } = false;

        public SubAccountPermissions? PermisosEmpleado { get; set; }
        public SystemAdminPermissions? PermisosAdminSistema { get; set; }

        // --- ACCESO GLOBAL ---
        public List<string> ModulosHabilitados { get; set; } = new();
        public List<string> FuncionalidadesExtra { get; set; } = new();
        public List<string> IdsRecursosExternos { get; set; } = new();

        public List<string> IdsPerfilesSociales { get; set; } = new();
    }
}