using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.Review;
using UserManagement.Application.Interfaces.Services;
using System.Collections.Generic; // Added for List<ReviewDetailDto>

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Class-level authorization, new public methods need [AllowAnonymous]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("No se pudo identificar al autor de la reseña.");
            string authorId = userIdClaim.Value;

            try
            {
                await _reviewService.CreateReviewAsync(authorId, dto);
                return Ok(new { message = "Reseña creada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet] // Route: /api/Reviews
        public async Task<IActionResult> GetReviews([FromQuery] string recipientId, [FromQuery] string contextId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsAsync(recipientId, contextId);
                return Ok(reviews);
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