using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.DTOs.Company.Employees;
using UserManagement.Application.DTOs.Public; // Added
using UserManagement.Application.Interfaces.Services;
using UserManagement.Domain.Enums;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyManagementController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyManagementController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // --- Existing Endpoints ---

        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (companyId == null) return Unauthorized("No se pudo identificar a la empresa.");
            
            try
            {
                var employeeId = await _companyService.CreateEmployeeAsync(companyId, dto);
                return Ok(new { message = "Empleado creado exitosamente", id = employeeId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        // --- New Employee Management Endpoints ---

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");

            try
            {
                var employees = await _companyService.GetEmployeesAsync(companyId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        [HttpGet("employees/{id}")]
        public async Task<IActionResult> GetEmployeeDetail(string id)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");

            try
            {
                var employee = await _companyService.GetEmployeeDetailAsync(companyId, id);
                return Ok(employee);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        [HttpPut("employees/{id}/permissions")]
        public async Task<IActionResult> UpdateEmployeePermissions(string id, [FromBody] UpdateEmployeePermissionsDto dto)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");
            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _companyService.UpdateEmployeePermissionsAsync(companyId, id, dto);
                return Ok(new { message = "Permisos del empleado actualizados." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        [HttpPut("employees/{id}/status")]
        public async Task<IActionResult> UpdateEmployeeStatus(string id, [FromBody] UpdateEmployeeStatusDto dto)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _companyService.UpdateEmployeeStatusAsync(companyId, id, dto.NuevoEstado);
                return Ok(new { message = $"Estado del empleado actualizado a {dto.NuevoEstado}." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        [HttpPut("employees/{id}/reset-password")]
        public async Task<IActionResult> ResetEmployeePassword(string id, [FromBody] ResetEmployeePasswordDto dto)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _companyService.ResetEmployeePasswordAsync(companyId, id, dto.NewPassword);
                return Ok(new { message = "La contraseña del empleado ha sido reseteada." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        [HttpPut("employees/{id}/profile")]
        public async Task<IActionResult> UpdateEmployeeProfile(string id, [FromBody] UpdateEmployeeProfileDto dto)
        {
            var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyId)) return Unauthorized("No se pudo identificar a la empresa.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _companyService.UpdateEmployeeProfileAsync(companyId, id, dto);
                return Ok(new { message = "Perfil del empleado actualizado." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }
        
        // --- Public Profile Endpoints ---

        [AllowAnonymous]
        [HttpGet("profiles/{companyId}/public")]
        public async Task<IActionResult> GetCompanyPublicProfiles(string companyId)
        {
            try
            {
                var profiles = await _companyService.GetCompanyPublicProfilesAsync(companyId);
                return Ok(profiles);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }
    }
}
