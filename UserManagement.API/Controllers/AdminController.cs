using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("pending-companies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
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
            var result = await _adminService.GetPendingMarketUsersAsync();
            return Ok(result);
        }

        [HttpPut("market/verify/{id}")]
        public async Task<IActionResult> VerifyMarketUser(string id)
        {
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