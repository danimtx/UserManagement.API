using Google.Cloud.Firestore;
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
        public string? CreadoPorId { get; set; }

        // --- PERFILES ESPECÍFICOS ---
        public PersonalProfile? DatosPersonales { get; set; }
        public CompanyProfile? DatosEmpresa { get; set; }

        // --- CONFIGURACIÓN DE PERMISOS (SEGÚN ROL) ---

        // A. Para Empleados de Empresa (SubCuentaEmpresa)
        public SubAccountPermissions? PermisosEmpleado { get; set; }

        // B. Para Administradores del Sistema (AdminSistema)
        public SystemAdminPermissions? PermisosAdminSistema { get; set; }

        // --- ACCESO A PRODUCTOS Y PAGOS ---
        // Módulos base activados: "Construccion", "Ferreteria", "Social", "Wallet"
        public List<string> ModulosHabilitados { get; set; } = new();

        // Funcionalidades PREMIUM (De pago): "AgenteIA_N8N", "ReportesAvanzados"
        public List<string> FuncionalidadesExtra { get; set; } = new();

        // Inventario de IDs externos (Redes sociales, Wallets externas)
        public List<string> IdsRecursosExternos { get; set; } = new();
    }
}