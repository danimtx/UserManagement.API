using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Google.Cloud.Firestore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly FirestoreDb _firestoreDb;

        // Constructor: Aquí pedimos el servicio (Inyección de Dependencias)
        public AdminController(IAdminService adminService, FirestoreDb firestoreDb)
        {
            _adminService = adminService;
            _firestoreDb = firestoreDb;
        }
        private async Task<bool> IsUserAdmin()
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return false;
            string uid = userIdClaim.Value;

            var doc = await _firestoreDb.Collection("users").Document(uid).GetSnapshotAsync();
            if (!doc.Exists) return false;

            var user = doc.ConvertTo<User>();

            return user.TipoUsuario == UserType.AdminSistema.ToString();
        }

        // =================================================
        // ENDPOINT: Ver lista de empresas pendientes
        // URL: GET /api/Admin/pending-companies
        // =================================================
        [HttpGet("pending-companies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado. Se requieren permisos de Administrador.");
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
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado. Se requieren permisos de Administrador.");

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