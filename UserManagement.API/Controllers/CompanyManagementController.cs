using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.Interfaces.Services;

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

        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            // Extraer ID del token (Seguridad)
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("No se pudo identificar a la empresa.");
            string companyId = userIdClaim.Value;

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

        [HttpPost("areas")]
        public async Task<IActionResult> AddArea([FromBody] string areaName)
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            string companyId = userIdClaim.Value;

            try
            {
                await _companyService.AddCompanyAreaAsync(companyId, areaName);
                return Ok(new { message = $"Área '{areaName}' agregada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
