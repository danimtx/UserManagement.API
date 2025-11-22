using System;
using System.Collections.Generic;
using System.Text;
using Google.Cloud.Firestore;

namespace UserManagement.Domain.Entities.ValueObjects
{
    /// <summary>
    /// Configuración de permisos para las Sub-Cuentas (Empleados).
    /// Define a qué módulos entran y qué pueden hacer dentro.
    /// </summary>
    [FirestoreData]
    public class SubAccountPermissions
    {
        // Clave (Key): Nombre del Módulo (ej: "Construccion", "Ferreteria", "Social")
        // Valor (Value): Objeto con el detalle de permisos
        [FirestoreProperty]
        public Dictionary<string, ModuleAccess> Modulos { get; set; } = new();
    }

    [FirestoreData]
    public class ModuleAccess
    {
        // ¿Puede entrar al módulo? 
        // Si es false, ni siquiera le aparece en el menú.
        [FirestoreProperty]
        public bool TieneAcceso { get; set; } = false;

        // Roles funcionales (ej: "ResidenteObra", "Contador", "CommunityManager")
        [FirestoreProperty]
        public List<string> RolesAsignados { get; set; } = new();

        // Permisos atómicos para acciones finas (ej: "ver_presupuesto", "aprobar_gasto")
        [FirestoreProperty]
        public List<string> PermisosEspecificos { get; set; } = new();

        // --- REQUISITO CLAVE ---
        // Mencionaste: "podrá seleccionar si darle permiso una cuenta de la red social o a ambas".
        // Aquí guardamos los IDs específicos a los que tiene permiso.
        // Ej: En módulo Social -> ["id_perfil_ferreteria"] (y no ve el de construcción)
        // Ej: En módulo Construcción -> ["id_proyecto_edificio_A"] (y no ve el Edificio B)
        [FirestoreProperty]
        public List<string> IdsRecursosPermitidos { get; set; } = new();
    }
}