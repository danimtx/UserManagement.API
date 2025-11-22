using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain.Entities
{
    [FirestoreData] // <--- ESTO LE DICE A FIRESTORE QUE PUEDE GUARDAR ESTA CLASE
    public class User
    {
        [FirestoreProperty] // <--- Indica que este campo se guarda en la BD
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Nombres { get; set; } = string.Empty;

        [FirestoreProperty]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? ApellidoMaterno { get; set; }

        [FirestoreProperty]
        public string Pais { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Departamento { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Profesion { get; set; } = string.Empty;
        [FirestoreProperty]
        public string TipoCuenta { get; set; } = string.Empty; // Ej: "Empresarial", "Personal", "Admin"

        [FirestoreProperty]
        public List<string> ModulosPermitidos { get; set; } = new List<string>();
    }
}
