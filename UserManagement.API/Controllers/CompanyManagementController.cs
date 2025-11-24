using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Company;
using UserManagement.Application.Interfaces;

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

        // POST: api/CompanyManagement/employees
        // Header: Authorization: Bearer <TOKEN_EMPRESA>
        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto, [FromQuery] string companyId)
        {
            // TODO: En producción, 'companyId' NO viene por Query, 
            // se extrae del Token JWT (User.Claims) por seguridad.
            // Por ahora lo pedimos en Query para probar en Swagger.

            if (string.IsNullOrEmpty(companyId))
                return BadRequest("Se requiere el ID de la empresa.");
            if (dto.FechaNacimiento.Kind != DateTimeKind.Utc)
            {
                dto.FechaNacimiento = dto.FechaNacimiento.ToUniversalTime();
            }

            if (dto.Permisos == null || dto.Permisos.Count == 0)
            {
                throw new Exception("Debe asignar al menos un permiso.");
            }

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

        // POST: api/CompanyManagement/areas
        [HttpPost("areas")]
        public async Task<IActionResult> AddArea([FromBody] string areaName, [FromQuery] string companyId)
        {
            if (string.IsNullOrEmpty(companyId)) return BadRequest("Falta companyId");

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
