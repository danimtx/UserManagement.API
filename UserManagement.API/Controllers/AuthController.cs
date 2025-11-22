using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos el servicio que definimos antes
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // =================================================
        // ENDPOINT: Registro de Persona Natural
        // URL: POST /api/auth/register/personal
        // =================================================
        [HttpPost("register/personal")]
        public async Task<IActionResult> RegisterPersonal([FromBody] RegisterPersonalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string userId = await _authService.RegisterPersonalAsync(dto);

                // Retornamos 201 Created y el ID del usuario creado
                return CreatedAtAction(nameof(Login), new { id = userId }, new { message = "Cuenta personal creada exitosamente", userId });
            }
            catch (Exception ex)
            {
                // En producción no debes devolver ex.Message directo por seguridad, 
                // pero para desarrollo es útil ver el error de Firebase.
                return BadRequest(new { error = ex.Message });
            }
        }

        // =================================================
        // ENDPOINT: Registro de Empresa
        // URL: POST /api/auth/register/company
        // =================================================
        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string userId = await _authService.RegisterCompanyAsync(dto);

                // Mensaje diferenciado para avisar que está pendiente de revisión
                return CreatedAtAction(nameof(Login), new { id = userId }, new
                {
                    message = "Cuenta de empresa registrada. Pendiente de validación por el Administrador.",
                    userId,
                    status = "Pendiente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // =================================================
        // ENDPOINT: Iniciar Sesión
        // URL: POST /api/auth/login
        // =================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // Esto lanzará la excepción "NotImplemented" que pusimos antes,
                // pero ya dejamos la ruta lista.
                string token = await _authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "El login desde backend está pendiente de configuración API Key. Usa login desde Frontend por ahora.");
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}