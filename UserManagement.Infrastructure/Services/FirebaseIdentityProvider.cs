using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.Infrastructure.Services
{
    public class FirebaseIdentityProvider : IIdentityProvider
    {
        private readonly FirebaseAuth _firebaseAuth;
        private readonly HttpClient _httpClient;
        private readonly string _webApiKey;

        public FirebaseIdentityProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
            _httpClient = httpClient;
            _webApiKey = configuration["Firebase:WebApiKey"] ?? throw new Exception("Falta Firebase:WebApiKey en appsettings");
        }

        public async Task<string> CreateUserAsync(string email, string password, string displayName)
        {
            var userArgs = new UserRecordArgs
            {
                Email = email,
                Password = password,
                DisplayName = displayName,
                Disabled = false
            };

            var userRecord = await _firebaseAuth.CreateUserAsync(userArgs);
            return userRecord.Uid;
        }

        public async Task DeleteUserAsync(string uid)
        {
            await _firebaseAuth.DeleteUserAsync(uid);
        }

        public async Task<(string Token, string Uid)> SignInAsync(string email, string password)
        {
            var authUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_webApiKey}";

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(authUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException($"Error de autenticación en Firebase: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>();

            if (result == null) throw new Exception("Respuesta vacía de Firebase.");

            return (result.idToken, result.localId);
        }

        private class FirebaseSignInResponse
        {
            public string idToken { get; set; } = string.Empty;
            public string localId { get; set; } = string.Empty;
        }
    }
}
