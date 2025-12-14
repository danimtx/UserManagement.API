using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.User;
using UserManagement.Application.DTOs.User.Profile;
using UserManagement.Application.DTOs.Public; // Added
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Class-level authorization, new public methods need [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISearchService _searchService; // Added

        public UsersController(IUserService userService, ISearchService searchService) // Modified constructor
        {
            _userService = userService;
            _searchService = searchService; // Added
        }

        [HttpGet("profile/me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            try
            {
                var userProfile = await _userService.GetUserProfileAsync(userId);
                return Ok(userProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado al obtener el perfil: {ex.Message}" });
            }
        }

        [HttpPut("profile/me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            try
            {
                await _userService.UpdateUserProfileAsync(userId, dto);
                return Ok(new { message = "Perfil actualizado exitosamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado al actualizar el perfil: {ex.Message}" });
            }
        }

        // --- New Public Endpoints ---

        [AllowAnonymous]
        [HttpGet("public/{userId}")]
        public async Task<IActionResult> GetPublicProfile(string userId)
        {
            try
            {
                var userProfile = await _userService.GetPublicProfileAsync(userId);
                return Ok(userProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado al obtener el perfil: {ex.Message}" });
            }
        }

        [AllowAnonymous]
        [HttpGet("public/search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] string? ciudad)
        {
            try
            {
                var searchResult = await _searchService.SearchAsync(q, ciudad);
                return Ok(searchResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado al realizar la búsqueda: {ex.Message}" });
            }
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