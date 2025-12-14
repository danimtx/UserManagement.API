using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.DTOs.Admin.Support; // Added
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminSistema,SuperAdminGlobal")] // Using hardcoded roles as per instruction style
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // --- Existing Endpoints for Pending Trays ---
        #region Pending Trays
        [HttpGet("identities/pending")]
        public async Task<IActionResult> GetPendingIdentities()
        {
            var result = await _adminService.GetPendingIdentitiesAsync();
            return Ok(result);
        }

        [HttpPut("identities/decision")]
        public async Task<IActionResult> MakeIdentityDecision([FromBody] IdentityDecisionDto dto)
        {
            try
            {
                await _adminService.DecideOnIdentityAsync(dto);
                return Ok(new { message = "Decisión procesada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // ... other tray endpoints
        #endregion

        // --- New Support & Moderation Endpoints ---

        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string term)
        {
            try
            {
                var result = await _adminService.SearchUsersAsync(term);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}"});
            }
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetail(string id)
        {
            try
            {
                var result = await _adminService.GetUserDetailAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}"});
            }
        }

        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> ChangeUserStatus(string id, [FromBody] AdminChangeStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _adminService.ChangeUserStatusAsync(id, dto);
                return Ok(new { message = "Estado del usuario actualizado." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}"});
            }
        }

        [HttpPut("users/{id}/reset-password")]
        public async Task<IActionResult> AdminResetUserPassword(string id, [FromBody] AdminResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            try
            {
                await _adminService.AdminResetUserPasswordAsync(id, dto.NewPassword);
                return Ok(new { message = "Contraseña del usuario reseteada." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}"});
            }
        }
    }
}