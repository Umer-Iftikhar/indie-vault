using IndieVault.Api.DTOs.Wishlist.Responses;
using IndieVault.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Player")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        [HttpPost("{gameId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Add(int gameId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                _logger.LogInformation("Attempting to add game with ID {GameId} to user {UserId}'s wishlist.", gameId, userId);
                await _wishlistService.AddToWishlistAsync(gameId, userId!);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Game {Gameid} is already in user {UserId}'s wishlist.", gameId, userId);
                return BadRequest(new { success = false, message = ex.Message });
            }

            _logger.LogInformation("Successfully added game with ID {GameId} to user {UserId}'s wishlist.", gameId, userId);
            return Created($"api/wishlist/{gameId}", new { success = true, message = "Game added to your wishlist." });
        }

        [HttpDelete("{gameId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Remove(int gameId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                _logger.LogInformation("Attempting to remove game with ID {GameId} from user {UserId}'s wishlist.", gameId, userId);
                await _wishlistService.RemoveFromWishlistAsync(gameId, userId!);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Failed to remove game with ID {GameId} from user {UserId}'s wishlist.", gameId, userId);
                return BadRequest(new { success = false, message = ex.Message });
            }

            _logger.LogInformation("Successfully removed game with ID {GameId} from user {UserId}'s wishlist.", gameId, userId);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<WishlistItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<WishlistItemDto>>> ViewWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlistGames = await _wishlistService.GetUserWishlistAsync(userId!);

            return Ok(wishlistGames);
        }
    }
}
