using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using UserManagement.Infrastructure.Persistence.Models;

namespace UserManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private const string UserCollectionName = "users";
        private const string ReviewCollectionName = "reviews";

        public UserRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        #region Mappers
        private UserDocument ToDocument(User entity)
        {
            var json = JsonConvert.SerializeObject(entity, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return JsonConvert.DeserializeObject<UserDocument>(json)!;
        }

        private User ToDomain(UserDocument doc)
        {
            var json = JsonConvert.SerializeObject(doc, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return JsonConvert.DeserializeObject<User>(json)!;
        }
        private ReviewDocument ToReviewDocument(Review entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            return JsonConvert.DeserializeObject<ReviewDocument>(json)!;
        }

        private Review ToReviewDomain(ReviewDocument doc)
        {
            var json = JsonConvert.SerializeObject(doc, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return JsonConvert.DeserializeObject<Review>(json)!;
        }
        #endregion

        public async Task AddAsync(User user)
        {
            var doc = ToDocument(user);
            doc.FechaRegistro = doc.FechaRegistro.ToUniversalTime();
            if (doc.DatosPersonales != null)
                doc.DatosPersonales.FechaNacimiento = doc.DatosPersonales.FechaNacimiento.ToUniversalTime();
            if (doc.DatosEmpresa?.Representante != null)
                doc.DatosEmpresa.Representante.FechaNacimiento = doc.DatosEmpresa.Representante.FechaNacimiento.ToUniversalTime();

            await _firestoreDb.Collection(UserCollectionName).Document(user.Id).SetAsync(doc);
        }

        public async Task UpdateAsync(User user)
        {
            await AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var docRef = await _firestoreDb.Collection(UserCollectionName).Document(id).GetSnapshotAsync();
            if (!docRef.Exists) return null;

            var document = docRef.ConvertTo<UserDocument>();
            return ToDomain(document);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var query = _firestoreDb.Collection(UserCollectionName).WhereEqualTo(nameof(UserDocument.Email), email).Limit(1);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;
            return ToDomain(snapshot.Documents[0].ConvertTo<UserDocument>());
        }
        
        public async Task<User?> GetByUserNameAsync(string username)
        {
            var query = _firestoreDb.Collection(UserCollectionName).WhereEqualTo(nameof(UserDocument.UserName), username).Limit(1);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;
            return ToDomain(snapshot.Documents[0].ConvertTo<UserDocument>());
        }

        public async Task<User?> GetByCiAsync(string ci)
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.DatosPersonales) + "." + nameof(PersonalProfileDocument.CI), ci)
                .Limit(1);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;
            return ToDomain(snapshot.Documents[0].ConvertTo<UserDocument>());
        }

        public async Task<User?> GetByNitAsync(string nit)
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.DatosEmpresa) + "." + nameof(CompanyProfileDocument.Nit), nit)
                .Limit(1);
            var snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;
            return ToDomain(snapshot.Documents[0].ConvertTo<UserDocument>());
        }

        public async Task<List<User>> GetEmployeesByCompanyAsync(string companyId)
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.CreadoPorId), companyId)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.SubCuentaEmpresa.ToString());
            
            var snapshot = await query.GetSnapshotAsync();
            
            return snapshot.Documents.Select(d => ToDomain(d.ConvertTo<UserDocument>())).ToList();
        }

        public async Task<List<User>> SearchUsersAsync(string query, string? ciudad)
        {
            string upperBound = query + "\uf8ff";

            var companiesQuery = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.Empresa.ToString())
                .WhereGreaterThanOrEqualTo(nameof(UserDocument.DatosEmpresa) + "." + nameof(CompanyProfileDocument.RazonSocial), query)
                .WhereLessThan(nameof(UserDocument.DatosEmpresa) + "." + nameof(CompanyProfileDocument.RazonSocial), upperBound);

            var personalQuery = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.Personal.ToString())
                .WhereGreaterThanOrEqualTo(nameof(UserDocument.DatosPersonales) + "." + nameof(PersonalProfileDocument.Profesion), query)
                .WhereLessThan(nameof(UserDocument.DatosPersonales) + "." + nameof(PersonalProfileDocument.Profesion), upperBound);

            var companiesTask = companiesQuery.GetSnapshotAsync();
            var personalTask = personalQuery.GetSnapshotAsync();

            await Task.WhenAll(companiesTask, personalTask);

            var allUsers = new List<User>();
            allUsers.AddRange(companiesTask.Result.Documents.Select(d => ToDomain(d.ConvertTo<UserDocument>())));
            allUsers.AddRange(personalTask.Result.Documents.Select(d => ToDomain(d.ConvertTo<UserDocument>())));
            
            if (!string.IsNullOrEmpty(ciudad))
            {
                allUsers = allUsers.Where(u => 
                    (u.DatosPersonales?.Departamento.Equals(ciudad, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.DatosEmpresa?.Sucursales.Any(s => s.Departamento.Equals(ciudad, StringComparison.OrdinalIgnoreCase)) ?? false)
                ).ToList();
            }

            return allUsers.DistinctBy(u => u.Id).ToList();
        }
        
        public async Task<List<User>> GetPendingCompaniesAsync()
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.Empresa.ToString())
                .WhereEqualTo(nameof(UserDocument.Estado), UserStatus.Pendiente.ToString());
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(d => ToDomain(d.ConvertTo<UserDocument>())).ToList();
        }
        
        public async Task<List<User>> GetCompaniesWithPendingModulesAsync()
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TieneSolicitudPendiente), true)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.Empresa.ToString())
                .WhereEqualTo(nameof(UserDocument.Estado), UserStatus.Activo.ToString());
            
            var snapshot = await query.GetSnapshotAsync();
            
            return snapshot.Documents
                .Select(d => ToDomain(d.ConvertTo<UserDocument>()))
                .Where(u => u.DatosEmpresa?.PerfilesComerciales.Any(p => p.Tipo == CommercialProfileType.Modulo && p.Estado == CommercialProfileStatus.Pendiente) ?? false)
                .ToList();
        }

        public async Task<List<User>> GetUsersWithPendingTagsAsync()
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TieneSolicitudPendiente), true)
                .WhereEqualTo(nameof(UserDocument.Estado), UserStatus.Activo.ToString());

            var snapshot = await query.GetSnapshotAsync();
            
            return snapshot.Documents
                .Select(d => ToDomain(d.ConvertTo<UserDocument>()))
                .Where(u => 
                    (u.DatosEmpresa?.PerfilesComerciales.Any(p => p.Tipo == CommercialProfileType.TagSocial && p.Estado == CommercialProfileStatus.Pendiente) ?? false) ||
                    (u.DatosPersonales?.Tags.Any(t => t.Estado == TagStatus.Pendiente) ?? false)
                )
                .ToList();
        }

        public async Task<List<User>> GetUsersWithPendingModuleRequestsAsync()
        {
            var query = _firestoreDb.Collection(UserCollectionName)
                .WhereEqualTo(nameof(UserDocument.TieneSolicitudPendiente), true)
                .WhereEqualTo(nameof(UserDocument.TipoUsuario), UserType.Personal.ToString())
                .WhereEqualTo(nameof(UserDocument.Estado), UserStatus.Activo.ToString());

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => ToDomain(d.ConvertTo<UserDocument>()))
                .Where(u => u.DatosPersonales?.SolicitudesModulos.Any(m => m.Estado == ModuleRequestStatus.Pendiente) ?? false)
                .ToList();
        }

        public async Task AddReviewAsync(Review review)
        {
            var doc = ToReviewDocument(review);
            doc.Timestamp = doc.Timestamp.ToUniversalTime();
            await _firestoreDb.Collection(ReviewCollectionName).Document(doc.Id).SetAsync(doc);
        }

        public async Task<bool> HasUserReviewedContextAsync(string authorId, string recipientId, string contextId)
        {
            var query = _firestoreDb.Collection(ReviewCollectionName)
                .WhereEqualTo(nameof(ReviewDocument.AuthorId), authorId)
                .WhereEqualTo(nameof(ReviewDocument.RecipientId), recipientId)
                .WhereEqualTo(nameof(ReviewDocument.ContextoId), contextId)
                .Limit(1);
                
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Count > 0;
        }

        public async Task<List<Review>> GetReviewsByContextAsync(string recipientId, string contextoId)
        {
            var query = _firestoreDb.Collection(ReviewCollectionName)
                .WhereEqualTo(nameof(ReviewDocument.RecipientId), recipientId)
                .WhereEqualTo(nameof(ReviewDocument.ContextoId), contextoId);
    
            var snapshot = await query.GetSnapshotAsync();
            
            return snapshot.Documents.Select(d => ToReviewDomain(d.ConvertTo<ReviewDocument>())).ToList();
        }
    }
}