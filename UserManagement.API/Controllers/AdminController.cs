using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        // Constructor: Aquí pedimos el servicio (Inyección de Dependencias)
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // =================================================
        // ENDPOINT: Ver lista de empresas pendientes
        // URL: GET /api/Admin/pending-companies
        // =================================================
        [HttpGet("pending-companies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            try
            {
                var result = await _adminService.GetPendingCompaniesAsync();

                if (result == null || result.Count == 0)
                {
                    return Ok(new { message = "No hay empresas pendientes de revisión." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // =================================================
        // ENDPOINT: Aprobar (Activar) una empresa
        // URL: PUT /api/Admin/approve-company/{id}
        // =================================================
        [HttpPut("approve-company/{id}")]
        public async Task<IActionResult> ApproveCompany(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID es obligatorio.");

            try
            {
                var message = await _adminService.ApproveCompanyAsync(id);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}