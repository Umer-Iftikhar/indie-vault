using IndieVault.Api.DTOs.Review.Requests;
using IndieVault.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpPost("/api/games/{gameId}/reviews")]
        [Authorize(Roles = "Player")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateReviewDto createReviewDto, int gameId)
        {
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Getting the user ID from claims
            bool alreadyReviewed = await _reviewService.HasUserReviewedAsync(gameId, currentUserId!);

            if (alreadyReviewed) // Double-checking on POST to prevent any bypass of the GET check (e.g., via direct POST request)
            {
                _logger.LogWarning("User {UserId} attempted to review game {GameId} again", currentUserId, gameId);
                return BadRequest(new { message = "You have already reviewed this game." });
            }

            var reviewId = await _reviewService.CreateReviewAsync(createReviewDto, currentUserId!, gameId);

            _logger.LogInformation("User {UserId} created a review for game {GameId}", currentUserId, gameId);
            return Created($"api/games/{gameId}/reviews", reviewId); 
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int reviewId)
        {
            try
            {
                // Authorization Check
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole("Admin");

                // Attempt to delete the review
                await _reviewService.DeleteReviewAsync(reviewId, currentUserId!, isAdmin);
                _logger.LogInformation("User {UserId} (IsAdmin: {IsAdmin}) deleted review {ReviewId}", currentUserId, isAdmin, reviewId);
            }
            catch(UnauthorizedAccessException ex) 
            {
                _logger.LogWarning(ex, "Unauthorized delete attempt on review {ReviewId} by user {UserId}", reviewId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Forbid();
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Delete failed: Review {ReviewId} not found", reviewId);
                return NotFound(new { message = "Review not found." });
            }
            return NoContent();
        }
    }
}
