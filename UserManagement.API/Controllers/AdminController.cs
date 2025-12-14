using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(UserType.AdminSistema)},{nameof(UserType.SuperAdminGlobal)}")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // --- Bandeja 1: Identidades ---
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

        // --- Bandeja 2: Módulos Técnicos ---
        [HttpGet("companies/modules/pending")]
        public async Task<IActionResult> GetPendingModules()
        {
            var result = await _adminService.GetPendingModulesAsync();
            return Ok(result);
        }

        [HttpPut("companies/modules/decision")]
        public async Task<IActionResult> MakeModuleDecision([FromBody] ModuleDecisionDto dto)
        {
            try
            {
                await _adminService.DecideOnModuleAsync(dto);
                return Ok(new { message = "Decisión de módulo procesada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // --- Bandeja 3: Tags y Reputación ---
        [HttpGet("tags/pending")]
        public async Task<IActionResult> GetPendingTags()
        {
            var result = await _adminService.GetPendingTagsAsync();
            return Ok(result);
        }

        [HttpPut("companies/tags/decision")]
        public async Task<IActionResult> MakeCompanyTagDecision([FromBody] CompanyTagDecisionDto dto)
        {
             try
            {
                await _adminService.DecideOnCompanyTagAsync(dto);
                return Ok(new { message = "Decisión de tag de empresa procesada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("personal/tags/decision")]
        public async Task<IActionResult> MakePersonalTagDecision([FromBody] PersonalTagDecisionDto dto)
        {
            try
            {
                await _adminService.DecideOnPersonalTagAsync(dto);
                return Ok(new { message = "Decisión de tag personal procesada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // --- Bandeja 4: Módulos Personales ---
        [HttpGet("personal/modules/pending")]
        public async Task<IActionResult> GetPendingPersonalModules()
        {
            var result = await _adminService.GetPendingPersonalModulesAsync();
            return Ok(result);
        }

        [HttpPut("personal/modules/decision")]
        public async Task<IActionResult> MakePersonalModuleDecision([FromBody] PersonalModuleDecisionDto dto)
        {
            try
            {
                await _adminService.DecideOnPersonalModuleAsync(dto);
                return Ok(new { message = "Decisión de módulo personal procesada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}