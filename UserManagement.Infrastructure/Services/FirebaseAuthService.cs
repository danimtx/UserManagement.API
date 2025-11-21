using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Text;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace UserManagement.Infrastructure.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly FirebaseAuth _firebaseAuth;
        private readonly FirestoreDb _firestoreDb;
        private readonly string _apiKey; // API Key Web para Login
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(IConfiguration configuration, HttpClient httpClient)
        {
            // Inicializamos instancias
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            // Importante: El ProjectId debe estar en tu appsettings.json
            _firestoreDb = FirestoreDb.Create(configuration["Firebase:ProjectId"]);
            _apiKey = configuration["Firebase:WebApiKey"]!;
            _httpClient = httpClient;
        }

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            // 1. Crear usuario en Firebase Auth
            var userArgs = new UserRecordArgs
            {
                Email = dto.Email,
                Password = dto.Password,
                DisplayName = $"{dto.Nombres} {dto.ApellidoPaterno}"
            };

            var userRecord = await _firebaseAuth.CreateUserAsync(userArgs);

            // 2. Guardar detalles en Firestore
            var userEntity = new User
            {
                Id = userRecord.Uid,
                Email = dto.Email,
                Nombres = dto.Nombres,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Pais = dto.Pais,
                Departamento = dto.Departamento,
                Profesion = dto.Profesion,
                RubrosHabilitados = new List<string>() { "general" }
            };

            DocumentReference docRef = _firestoreDb.Collection("users").Document(userRecord.Uid);
            await docRef.SetAsync(userEntity);

            return userRecord.Uid;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // 1. Validar password usando la API REST de Google (El SDK Admin no hace esto)
            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
            var payload = new
            {
                email = dto.Email,
                password = dto.Password,
                returnSecureToken = true
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic authData = JsonConvert.DeserializeObject(responseString)!;
            string uid = authData.localId;

            // 2. Verificar si tiene permiso en el Rubro (Firestore)
            DocumentSnapshot snapshot = await _firestoreDb.Collection("users").Document(uid).GetSnapshotAsync();

            if (!snapshot.Exists) throw new Exception("El usuario existe en Auth pero no tiene datos en Firestore.");

            User user = snapshot.ConvertTo<User>();

            // Verificar rubro (ignoramos mayúsculas/minúsculas)
            if (!user.RubrosHabilitados.Any(r => r.Equals(dto.RubroSeleccionado, StringComparison.OrdinalIgnoreCase))
                && dto.RubroSeleccionado.ToLower() != "general")
            {
                throw new UnauthorizedAccessException($"No tienes acceso al rubro: {dto.RubroSeleccionado}");
            }

            // 3. Generar Token Personalizado para devolver al frontend
            var claims = new Dictionary<string, object>
            {
                { "rubro_actual", dto.RubroSeleccionado },
                { "profesion", user.Profesion }
            };

            string customToken = await _firebaseAuth.CreateCustomTokenAsync(uid, claims);
            return customToken;
        }
    }
}
