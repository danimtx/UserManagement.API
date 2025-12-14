using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("tags/request")]
        public async Task<IActionResult> RequestTag([FromBody] RequestTagDto dto)
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("No se pudo identificar al usuario.");
            string userId = userIdClaim.Value;

            try
            {
                await _userService.RequestTagAsync(userId, dto);
                return Ok(new { message = $"Solicitud para el tag '{dto.TagNombre}' enviada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("modules/request")]
        public async Task<IActionResult> RequestModule([FromBody] RequestPersonalModuleDto dto)
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("No se pudo identificar al usuario.");
            string userId = userIdClaim.Value;

            try
            {
                await _userService.RequestModuleAsync(userId, dto);
                return Ok(new { message = $"Solicitud para el módulo '{dto.NombreModulo}' enviada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
