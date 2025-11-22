using Google.Cloud.Firestore;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities
{
    /// <summary>
    /// Entidad Raíz. Representa un documento en la colección 'users' de Firestore.
    /// Ahora soporta Empresas, Personas y Subcuentas de forma estructurada.
    /// </summary>
    [FirestoreData]
    public class User
    {
        // --- Identificación Básica (Común para todos) ---

        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        // "Empresa", "Personal", "SubCuenta" (Mapeado desde el Enum UserType)
        [FirestoreProperty]
        public string TipoUsuario { get; set; } = UserType.Personal.ToString();

        // "Activo", "Pendiente", "Suspendido"
        [FirestoreProperty]
        public string Estado { get; set; } = UserStatus.Activo.ToString();

        [FirestoreProperty]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;


        // ==========================================
        // DATOS DE PERFIL (Lógica Polimórfica)
        // ==========================================

        // Opción A: Si es Persona Natural (Tus antiguos campos Nombres, Profesion, etc. viven aquí)
        [FirestoreProperty]
        public PersonalProfile? DatosPersonales { get; set; }

        // Opción B: Si es Empresa (Razón Social, NIT, Representante Legal)
        [FirestoreProperty]
        public CompanyProfile? DatosEmpresa { get; set; }


        // ==========================================
        // GESTIÓN DE SUB-CUENTAS (Empleados)
        // ==========================================

        // Solo para empleados: ID de la empresa dueña
        [FirestoreProperty]
        public string? CuentaPadreId { get; set; }

        // Solo para empleados: Qué puede hacer exactamente (Roles y Permisos)
        [FirestoreProperty]
        public SubAccountPermissions? PermisosEmpleado { get; set; }


        // ==========================================
        // ACCESO GLOBAL AL SISTEMA
        // ==========================================

        // Qué productos compró/tiene el usuario (ej: "Construccion", "Wallet")
        [FirestoreProperty]
        public List<string> ModulosHabilitados { get; set; } = new();

        // IDs para la integración con la Red Social
        [FirestoreProperty]
        public List<string> IdsPerfilesSociales { get; set; } = new();
    }
}