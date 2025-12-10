using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/personal")]
        public async Task<IActionResult> RegisterPersonal([FromBody] RegisterPersonalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                string userId = await _authService.RegisterPersonalAsync(dto);
                return Ok(new { message = "Cuenta personal creada exitosamente", userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register/company")]
        public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                string userId = await _authService.RegisterCompanyAsync(dto);
                return Ok(new
                {
                    message = "Empresa registrada. Pendiente de validación.",
                    userId,
                    status = "Pendiente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

                [HttpPost("login")]

                public async Task<IActionResult> Login([FromBody] LoginDto dto)

                {

                    try

                    {

                        var result = await _authService.LoginAsync(dto);

                        return Ok(result);

                    }

                    catch (UnauthorizedAccessException ex)

                    {

                        return Unauthorized(new { error = ex.Message });

                    }

                    catch (Exception ex)

                    {

                        return BadRequest(new { error = ex.Message });

                    }

                }

        

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

            }

        }

        