


# PLAN DE IMPLEMENTACIÓN: MECANISMO DE REFRESH TOKEN

## 1. Objetivo y Concepto
Implementar un sistema de renovación de sesión seguro ("Refresh Token") para evitar que el usuario tenga que introducir su contraseña cada hora.

**El Flujo de Seguridad:**
1.  **Login:** El usuario recibe dos llaves:
    * `AccessToken` (Caduca en 1 hora): Usado para todas las peticiones a la API.
    * `RefreshToken` (Larga duración): Usado **solo** para pedir nuevos AccessTokens.
2.  **Almacenamiento (Cliente):**
    * El `RefreshToken` debe guardarse en **Secure Storage** (Móvil) o **HttpOnly Cookie** (Web). Nunca en LocalStorage accesible por JS.
3.  **Renovación:** Cuando la API responde `401 Unauthorized`, el cliente envía el `RefreshToken` al endpoint `/refresh-token` y recibe credenciales nuevas.

---

## 2. Modificaciones en la Capa de Infraestructura
**Archivo:** `UserManagement.Infrastructure/Services/FirebaseIdentityProvider.cs`

**Tarea:** Capturar el token de refresco que Firebase ya nos da y añadir la lógica para canjearlo.

1.  **Actualizar clase de respuesta interna:**
    Dentro de `FirebaseIdentityProvider`, modificar `FirebaseSignInResponse` para incluir las propiedades que faltan.
    ```csharp
    private class FirebaseSignInResponse
    {
        public string idToken { get; set; } = string.Empty;
        public string localId { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty; // <--- NUEVO
        public string expiresIn { get; set; } = string.Empty;    // <--- NUEVO
    }
    ```

2.  **Actualizar método `SignInAsync`:**
    Debe devolver una tupla con 3 valores: `(string Token, string Uid, string RefreshToken)`.
    * *Acción:* Mapear `result.refreshToken` del JSON de respuesta.

3.  **Implementar nuevo método `RefreshTokenAsync`:**
    ```csharp
    public async Task<(string NewToken, string NewRefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var authUrl = $"[https://securetoken.googleapis.com/v1/token?key=](https://securetoken.googleapis.com/v1/token?key=){_webApiKey}";
        var payload = new { grant_type = "refresh_token", refresh_token = refreshToken };

        var response = await _httpClient.PostAsJsonAsync(authUrl, payload);
        if (!response.IsSuccessStatusCode) throw new UnauthorizedAccessException("Refresh Token inválido.");

        var result = await response.Content.ReadFromJsonAsync<FirebaseRefreshTokenResponse>();
        return (result.id_token, result.refresh_token);
    }

    private class FirebaseRefreshTokenResponse 
    { 
        public string id_token { get; set; } 
        public string refresh_token { get; set; } 
    }
    ```

---

## 3. Modificaciones en la Capa de Aplicación (DTOs e Interfaces)

**Archivo:** `UserManagement.Application/Interfaces/Services/IIdentityProvider.cs`
* Actualizar firma: `Task<(string Token, string Uid, string RefreshToken)> SignInAsync(...)`
* Agregar: `Task<(string NewToken, string NewRefreshToken)> RefreshTokenAsync(string refreshToken);`

**Archivo:** `UserManagement.Application/Interfaces/Services/IAuthService.cs`
* Cambiar retorno de Login: `Task<AuthResponseDto> LoginAsync(...)`
* Agregar: `Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);`

**Nuevos DTOs:**
Crear en `UserManagement.Application/DTOs/Auth/`:

1.  **`AuthResponseDto.cs`**
    ```csharp
    public class AuthResponseDto {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
    }
    ```

2.  **`RefreshTokenRequestDto.cs`**
    ```csharp
    public class RefreshTokenRequestDto {
        public string RefreshToken { get; set; }
    }
    ```

---

## 4. Modificaciones en la Lógica de Negocio (Service)
**Archivo:** `UserManagement.Application/Services/AuthService.cs`

1.  **Actualizar `LoginAsync`:**
    ```csharp
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Obtener los 3 valores del provider
        var (token, uid, refreshToken) = await _identityProvider.SignInAsync(loginDto.Email, loginDto.Password);
        
        // ... (Mantener lógica existente de validación de usuario y módulos) ...

        return new AuthResponseDto 
        { 
            Token = token, 
            RefreshToken = refreshToken, 
            UserId = uid 
        };
    }
    ```

2.  **Implementar `RefreshTokenAsync`:**
    ```csharp
    public async Task<AuthResponseDto> RefreshTokenAsync(string incomingRefreshToken)
    {
        var (newToken, newRefreshToken) = await _identityProvider.RefreshTokenAsync(incomingRefreshToken);
        
        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken, // Firebase rota el token, usar el nuevo
            UserId = "" // Opcional: decodificar del token si se necesita
        };
    }
    ```

---

## 5. Modificaciones en la API (Controller)
**Archivo:** `UserManagement.API/Controllers/AuthController.cs`

1.  **Actualizar `Login`:**
    Cambiar el tipo de retorno para devolver el objeto completo `AuthResponseDto`.

2.  **Nuevo Endpoint:**
    ```csharp
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Sesión expirada. Inicie sesión nuevamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    ```