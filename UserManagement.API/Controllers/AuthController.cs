using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Constructor: Inyectamos la interfaz IAuthService que definimos en la Capa de Aplicación
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            // 1. Validación rápida: ¿Coinciden las contraseñas?
            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new { message = "Las contraseñas no coinciden." });
            }

            try
            {
                // 2. Llamamos al servicio (Infrastructure -> Firebase)
                var userId = await _authService.RegisterAsync(dto);

                // 3. Retornamos éxito (200 OK)
                return Ok(new { message = "Usuario registrado exitosamente", userId = userId });
            }
            catch (Exception ex)
            {
                // Si algo falla (ej: el correo ya existe), retornamos BadRequest (400)
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // 1. Intentamos loguear y obtener el token
                var token = await _authService.LoginAsync(dto);

                // 2. Retornamos el token (200 OK)
                return Ok(new { token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Si la contraseña está mal o el rubro no es permitido -> 401 Unauthorized
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Otros errores -> 400 BadRequest
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}