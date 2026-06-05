using IndieVault.Api.DTOs.Game.Requests;
using IndieVault.Api.DTOs.Game.Responses;
using IndieVault.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "GameDev")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("formdata")]
        [ProducesResponseType(typeof(GameFormDataDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameFormDataDto>> GetFormData()
        {
            var formData = await _gameService.GetFormDataAsync();
            return Ok(formData);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Upload([FromForm] GameUploadDto gameUploadDto)
        {
            // Validation: Max 5 screenshots
            if (gameUploadDto.Screenshots != null && gameUploadDto.Screenshots.Count > 5)
            {
                return BadRequest(new { message = "Maximum 5 screenshots allowed." });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            var gameId = await _gameService.UploadGameAsync(gameUploadDto, currentUserId);

            return Created($"api/games/{gameId}", new { id = gameId });
        }


        [HttpGet("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MyGameDto>>> MyGames()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var games = await _gameService.GetMyGamesAsync(currentUserId!);

            return Ok (games);
        }

        [HttpDelete("{gameId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int gameId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _gameService.DeleteGameAsync(gameId, currentUserId!);
            }
            catch (KeyNotFoundException ex) {
                return NotFound(new { message = "Game not found." });
            }
            catch (UnauthorizedAccessException ex) {
                return Forbid();
            }

            return NoContent();
        }


        [HttpPut("{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Edit(int gameId, GameUpdateDto gameUpdateDto)
        {
            
            // Validation: Max 5 screenshots
            if (gameUpdateDto.Screenshots != null && gameUpdateDto.Screenshots.Count > 5)
            {
                return BadRequest(new { message = "Maximum 5 screenshots allowed." });
            }
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _gameService.UpdateGameAsync(gameUpdateDto, currentUserId!, gameId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Game not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }

            return Ok(new { id = gameId, message = "Game updated successfully." });
        }

        [AllowAnonymous]
        [HttpGet("{gameId}")]
        [ProducesResponseType(typeof(GameDetailDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameDetailDto>> Details(int gameId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var game = await _gameService.GetGameDetailsAsync(gameId, userId ?? string.Empty);

            return Ok(game);
        }
    }
}
