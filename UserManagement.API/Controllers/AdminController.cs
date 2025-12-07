using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.Application.DTOs.Admin;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;
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
        private readonly IUserRepository _userRepository;

        public AdminController(IAdminService adminService, IUserRepository userRepository)
        {
            _adminService = adminService;
            _userRepository = userRepository;
        }
        private async Task<bool> IsUserAdmin()
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return false;
            string uid = userIdClaim.Value;

            var user = await _userRepository.GetByIdAsync(uid);
            if (user == null) return false;

            return user.TipoUsuario == UserType.AdminSistema.ToString() ||
                   user.TipoUsuario == UserType.SuperAdminGlobal.ToString();
        }

        [HttpGet("pending-companies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado.");

            try
            {
                var result = await _adminService.GetPendingCompaniesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("approve-company/{id}")]
        public async Task<IActionResult> ApproveCompany(string id)
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado.");

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

        [HttpPut("process-modules")]
        public async Task<IActionResult> ProcessModules([FromBody] ApproveModuleDto dto)
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado.");

            try
            {
                var msg = await _adminService.ProcessModuleRequestsAsync(dto);
                return Ok(new { message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("market/pending")]
        public async Task<IActionResult> GetPendingMarket()
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado.");

            var result = await _adminService.GetPendingMarketUsersAsync();
            return Ok(result);
        }

        [HttpPut("market/verify/{id}")]
        public async Task<IActionResult> VerifyMarketUser(string id)
        {
            if (!await IsUserAdmin()) return Unauthorized("Acceso denegado.");

            try
            {
                var msg = await _adminService.VerifyMarketUserAsync(id);
                return Ok(new { message = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}