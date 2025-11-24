using Google.Cloud.Firestore;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities
{
    //entidad raiz, soporta diferentes tipos de usuario: Admin, Empresa, SubCuenta(Empleado), Profesional Independiente
    [FirestoreData]
    public class User
    {
        [FirestoreProperty] public string Id { get; set; } = string.Empty;
        [FirestoreProperty] public string Email { get; set; } = string.Empty;
        [FirestoreProperty] public string TipoUsuario { get; set; } = string.Empty;
        [FirestoreProperty] public string Estado { get; set; } = string.Empty;
        [FirestoreProperty] public DateTime FechaRegistro { get; set; }

        // --- PERFILES ---
        // Si es SubCuenta, usaremos DatosPersonales para guardar su Nombre, CI, Dirección, Celular.
        [FirestoreProperty]
        public PersonalProfile? DatosPersonales { get; set; }
        [FirestoreProperty]
        public CompanyProfile? DatosEmpresa { get; set; }


        // --- SUB-CUENTAS (Empleados) ---
        [FirestoreProperty]
        public string? CuentaPadreId { get; set; }
        [FirestoreProperty]
        public string? AreaTrabajo { get; set; }//área dentro de la empresa (ej: "Administración", "Obras", "Ventas")
        [FirestoreProperty]
        public bool EsSuperAdminEmpresa { get; set; } = false; //campo para identificar al segundo al mando de la empresa
        [FirestoreProperty]
        public SubAccountPermissions? PermisosEmpleado { get; set; }


        // --- ACCESO GLOBAL ---
        [FirestoreProperty]
        public List<string> ModulosHabilitados { get; set; } = new();
        // IDs para la integración con la Red Social
        [FirestoreProperty]
        public List<string> IdsPerfilesSociales { get; set; } = new();
    }
}