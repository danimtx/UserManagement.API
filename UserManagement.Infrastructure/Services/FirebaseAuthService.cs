using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json; 
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.Interfaces;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly FirebaseAuth _firebaseAuth;
        private readonly FirestoreDb _firestoreDb;
        private readonly HttpClient _httpClient; // Cliente para llamar a la API REST de Google
        private readonly string _webApiKey;      // Tu llave pública (AIzaSy...)
        private const string CollectionName = "users";

        // Inyección de dependencias actualizada
        public FirebaseAuthService(FirestoreDb firestoreDb, HttpClient httpClient, IConfiguration configuration)
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _firestoreDb = firestoreDb;
            _httpClient = httpClient;
            // Leemos la API Key desde tu archivo appsettings.json
            _webApiKey = configuration["Firebase:WebApiKey"];
        }

        // =========================================================
        // 1. REGISTRO DE PERSONAS NATURALES (Tu código intacto)
        // =========================================================
        public async Task<string> RegisterPersonalAsync(RegisterPersonalDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                // Lanzamos una excepción que el Controlador capturará y devolverá como BadRequest
                throw new ArgumentException("Las contraseñas ingresadas no coinciden.");
            }
            // A. Crear usuario en Firebase Authentication (Solo Credenciales)
            var userArgs = new UserRecordArgs
            {
                Email = dto.Email,
                Password = dto.Password,
                DisplayName = $"{dto.Nombres} {dto.ApellidoPaterno}",
                Disabled = false // Nace activo
            };

            UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            string uid = userRecord.Uid;

            // B. Preparar la Entidad de Dominio (Todos los datos complejos)
            var newUser = new User
            {
                Id = uid,
                Email = dto.Email,
                TipoUsuario = UserType.Personal.ToString(),
                Estado = UserStatus.Activo.ToString(), // Personas entran directo
                FechaRegistro = DateTime.UtcNow,

                // Llenamos SOLO el perfil personal
                DatosPersonales = new PersonalProfile
                {
                    Nombres = dto.Nombres,
                    ApellidoPaterno = dto.ApellidoPaterno,
                    ApellidoMaterno = dto.ApellidoMaterno ?? "",
                    FechaNacimiento = dto.FechaNacimiento,
                    CI = dto.CI,
                    Pais = dto.Pais,
                    Departamento = dto.Departamento,
                    Direccion = dto.Direccion,
                    Celular = dto.Celular,
                    Profesion = dto.Profesion,
                    LinkedInUrl = dto.LinkedInUrl,
                    Nit = dto.Nit,
                    FotoCiUrl = dto.FotoCiUrl,
                    FotoTituloUrl = dto.FotoTituloUrl,
                    // Por defecto false hasta que Admin revise (si quiere vender)
                    VerificadoMarket = false
                },

                // Módulos por defecto para una persona
                ModulosHabilitados = new List<string> { "Social", "Wallet", "Market" }
            };

            // C. Guardar en Firestore
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(uid);
            await docRef.SetAsync(newUser);

            return uid;
        }

        // =========================================================
        // 2. REGISTRO DE EMPRESAS (Tu código intacto)
        // =========================================================
        public async Task<string> RegisterCompanyAsync(RegisterCompanyDto dto)
        {
            // A. Crear usuario en Authentication
            var userArgs = new UserRecordArgs
            {
                Email = dto.EmailEmpresa,
                Password = dto.Password,
                DisplayName = dto.RazonSocial,
                Disabled = false // El login lo permite, pero el backend bloqueará por estado "Pendiente"
            };

            UserRecord userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            string uid = userRecord.Uid;

            // B. Preparar la Entidad (Datos Empresariales)
            var newUser = new User
            {
                Id = uid,
                Email = dto.EmailEmpresa,
                TipoUsuario = UserType.Empresa.ToString(),
                Estado = UserStatus.Pendiente.ToString(), // <--- IMPORTANTE: Nace Pendiente
                FechaRegistro = DateTime.UtcNow,

                // Llenamos SOLO el perfil de empresa
                DatosEmpresa = new CompanyProfile
                {
                    RazonSocial = dto.RazonSocial,
                    TipoEmpresa = dto.TipoEmpresa,
                    Nit = dto.Nit,
                    Pais = dto.Pais,
                    Departamento = dto.Departamento,
                    DireccionEscrita = dto.DireccionEscrita,
                    Latitud = dto.Latitud,
                    Longitud = dto.Longitud,
                    TelefonoFijo = dto.TelefonoFijo,
                    CelularCorporativo = dto.CelularCorporativo,

                    // Mapeo del Representante Legal
                    Representante = new LegalRepresentative
                    {
                        Nombres = dto.Representante.Nombres,
                        ApellidoPaterno = dto.Representante.ApellidoPaterno,
                        ApellidoMaterno = dto.Representante.ApellidoMaterno,
                        CI = dto.Representante.CI,
                        FechaNacimiento = dto.Representante.FechaNacimiento,
                        DireccionDomicilio = dto.Representante.DireccionDomicilio,
                        EmailPersonal = dto.Representante.EmailPersonal,
                        NumerosContacto = dto.Representante.NumerosContacto
                    },

                    // Diccionario de documentos (URLs)
                    DocumentosLegales = dto.DocumentosUrls
                },

                // Módulos que la empresa SOLICITA (El admin luego decidirá si los aprueba todos)
                ModulosHabilitados = dto.ModulosSolicitados
            };

            // C. Guardar en Firestore
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(uid);
            await docRef.SetAsync(newUser);

            return uid;
        }

        // =========================================================
        // 3. LOGIN REAL (IMPLEMENTACIÓN NUEVA)
        // =========================================================
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // PASO A: Autenticar contra la API REST de Google Identity Toolkit
            // Esto verifica que el Email y Password sean correctos y devuelve el UID
            var authUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";

            var payload = new
            {
                email = loginDto.Email,
                password = loginDto.Password,
                returnSecureToken = true
            };

            // Hacemos la petición HTTP POST
            var response = await _httpClient.PostAsJsonAsync(authUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                // Si falla (ej: contraseña incorrecta), lanzamos error
                throw new Exception("Credenciales inválidas. Verifique su correo y contraseña.");
            }

            // Leemos la respuesta para obtener el UID y el Token temporal
            var authResult = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
            string uid = authResult.localId;
            string token = authResult.idToken;

            // PASO B: Verificar estado en Firestore (REGLAS DE NEGOCIO)
            // Buscamos el documento del usuario por su UID
            DocumentSnapshot doc = await _firestoreDb.Collection(CollectionName).Document(uid).GetSnapshotAsync();

            if (!doc.Exists)
            {
                throw new Exception("Usuario autenticado correctamente, pero no se encontraron sus datos en el sistema.");
            }

            // Convertimos el documento Firestore a tu clase User
            User user = doc.ConvertTo<User>();

            // --- VALIDACIONES DE ESTADO ---

            // REGLA 1: Si es Empresa y está Pendiente -> BLOQUEAR ACCESO
            if (user.TipoUsuario == UserType.Empresa.ToString() &&
                user.Estado == UserStatus.Pendiente.ToString())
            {
                throw new UnauthorizedAccessException("Su cuenta de empresa está en revisión. Espere la aprobación del Administrador.");
            }

            // REGLA 2: Si está Suspendido o Rechazado -> BLOQUEAR ACCESO
            if (user.Estado == UserStatus.Suspendido.ToString() ||
                user.Estado == UserStatus.Rechazado.ToString())
            {
                throw new UnauthorizedAccessException($"Su cuenta está {user.Estado}. Contacte a soporte.");
            }

            // REGLA 3: Validación de Módulo (Opcional pero recomendada)
            if (!string.IsNullOrEmpty(loginDto.ModuloSeleccionado))
            {
                // Verificamos si el usuario tiene habilitado ese módulo en su lista
                if (user.ModulosHabilitados != null && !user.ModulosHabilitados.Contains(loginDto.ModuloSeleccionado))
                {
                    throw new UnauthorizedAccessException($"No tienes permiso para acceder al módulo: {loginDto.ModuloSeleccionado}.");
                }
            }

            // Si pasa todas las validaciones, retornamos el Token JWT
            return token;
        }

        // Clase privada auxiliar para mapear la respuesta JSON de Google
        private class FirebaseAuthResponse
        {
            public string localId { get; set; } // El UID del usuario
            public string idToken { get; set; } // El Token JWT
        }
    }
}