using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "users";

        public AdminService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // 1. OBTENER PENDIENTES
        public async Task<List<CompanyValidationDto>> GetPendingCompaniesAsync()
        {
            // Consulta: Dame usuarios donde TipoUsuario == "Empresa" Y Estado == "Pendiente"
            Query query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("TipoUsuario", UserType.Empresa.ToString())
                .WhereEqualTo("Estado", UserStatus.Pendiente.ToString());

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            var pendingCompanies = new List<CompanyValidationDto>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    // Convertimos el documento a nuestra entidad User completa
                    User user = document.ConvertTo<User>();

                    // Mapeamos solo lo que el Admin necesita ver en el DTO
                    pendingCompanies.Add(new CompanyValidationDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Estado = user.Estado,
                        FechaRegistro = user.FechaRegistro,
                        // Extraemos datos del perfil de empresa
                        RazonSocial = user.DatosEmpresa?.RazonSocial ?? "Sin Nombre",
                        Nit = user.DatosEmpresa?.Nit ?? "S/N",
                        DocumentosLegales = user.DatosEmpresa?.DocumentosLegales ?? new Dictionary<string, string>(),
                        NombreRepresentante = $"{user.DatosEmpresa?.Representante?.Nombres} {user.DatosEmpresa?.Representante?.ApellidoPaterno}"
                    });
                }
            }

            return pendingCompanies;
        }

        // 2. APROBAR EMPRESA
        public async Task<string> ApproveCompanyAsync(string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(userId);

            // Verificamos que exista antes de actualizar
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) throw new Exception("La empresa no existe.");

            // Actualizamos SOLO el campo Estado a "Activo"
            var updates = new Dictionary<string, object>
            {
                { "Estado", UserStatus.Activo.ToString() }
            };

            await docRef.UpdateAsync(updates);

            return $"La empresa con ID {userId} ha sido aprobada y activada correctamente.";
        }
    }
}