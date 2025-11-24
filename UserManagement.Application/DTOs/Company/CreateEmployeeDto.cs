using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Application.DTOs.Company
{
    public class CreateEmployeeDto
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordTemporal { get; set; } = string.Empty;

        // --- Datos Personales del Empleado ---
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string CI { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string DireccionEscrita { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;

        // --- Rol en la Empresa ---
        public string AreaTrabajo { get; set; } = string.Empty; // Ej: "Marketing"
        public bool EsSuperAdmin { get; set; } = false; // "Segundo al mando"

        // --- Matriz de Permisos ---
        // Clave: "Construccion", "Ferreteria", "Social", "Herramientas"
        public Dictionary<string, ModuleAccessDto> Permisos { get; set; } = new();
    }

    public class ModuleAccessDto
    {
        public bool Acceso { get; set; }
        // Ej: ["Presobras", "Proveedores"]
        public List<string> Funcionalidades { get; set; } = new();
        // Ej: IDs de perfiles sociales
        public List<string> RecursosIds { get; set; } = new();
    }
}
