using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Infrastructure.Persistence.Models;

namespace UserManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "users";

        public UserRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        private UserDocument ToDocument(User entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            return JsonConvert.DeserializeObject<UserDocument>(json)!;
        }

        private User ToDomain(UserDocument doc)
        {
            var json = JsonConvert.SerializeObject(doc);
            return JsonConvert.DeserializeObject<User>(json)!;
        }

        public async Task AddAsync(User user)
        {
            var doc = ToDocument(user);
            if (doc.FechaRegistro.Kind != DateTimeKind.Utc)
                doc.FechaRegistro = doc.FechaRegistro.ToUniversalTime();
            if (doc.DatosPersonales != null)
                doc.DatosPersonales.FechaNacimiento = doc.DatosPersonales.FechaNacimiento.ToUniversalTime();

            await _firestoreDb.Collection(CollectionName).Document(user.Id).SetAsync(doc);
        }

        public async Task UpdateAsync(User user)
        {
            await AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var docRef = await _firestoreDb.Collection(CollectionName).Document(id).GetSnapshotAsync();
            if (!docRef.Exists) return null;

            var document = docRef.ConvertTo<UserDocument>();
            return ToDomain(document);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var query = _firestoreDb.Collection(CollectionName).WhereEqualTo("Email", email).Limit(1);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0) return null;

            var document = snapshot.Documents[0].ConvertTo<UserDocument>();
            return ToDomain(document);
        }

        public async Task<List<User>> GetPendingCompaniesAsync()
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("TipoUsuario", UserType.Empresa.ToString())
                .WhereEqualTo("Estado", UserStatus.Pendiente.ToString());

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => ToDomain(d.ConvertTo<UserDocument>()))
                .ToList();
        }

        public async Task<List<User>> GetActivePersonalUsersAsync()
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("TipoUsuario", UserType.Personal.ToString())
                .WhereEqualTo("Estado", UserStatus.Activo.ToString());

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => ToDomain(d.ConvertTo<UserDocument>()))
                .ToList();
        }
        public async Task<User?> GetByUserNameAsync(string username)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("UserName", username)
                .Limit(1);

            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0) return null;

            var document = snapshot.Documents[0].ConvertTo<UserDocument>();
            return ToDomain(document);
        }
    }
}
