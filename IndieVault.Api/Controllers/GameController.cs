using IndieVault.Api.DTOs.Game.Requests;
using IndieVault.Api.DTOs.Game.Responses;
using IndieVault.Api.Enums;
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
        private readonly IGameBrowseService _gameBrowseService;
        private readonly ILogger<GameController> _logger;
        public GameController(IGameService gameService, IGameBrowseService gameBrowseService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _gameBrowseService = gameBrowseService;
            _logger = logger;
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
                _logger.LogInformation("Attempting to delete game with ID {GameId}", gameId);
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _gameService.DeleteGameAsync(gameId, currentUserId!);
            }
            catch (KeyNotFoundException ex) {
                _logger.LogWarning(ex,"Delete failed: Game {GameId} not found", gameId);

                return NotFound(new { message = "Game not found." });
            }
            catch (UnauthorizedAccessException ex) {
                _logger.LogWarning(ex,"Unauthorized delete attempt on game {GameId} by user {UserId}", gameId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Forbid();
            }

            _logger.LogInformation("Game with ID {GameId} deleted successfully", gameId);
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
                _logger.LogInformation("Attempting to update game with ID {GameId}", gameId);
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _gameService.UpdateGameAsync(gameUpdateDto, currentUserId!, gameId);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Update failed: Game {GameId} not found", gameId);
                return NotFound(new { message = "Game not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized update attempt on game {GameId} by user {UserId}", gameId, User.FindFirstValue(ClaimTypes.NameIdentifier));
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

            _logger.LogInformation("Fetching details for game with ID {GameId} by user {UserId}", gameId, userId ?? "Anonymous");

            var game = await _gameService.GetGameDetailsAsync(gameId, userId ?? string.Empty);

            return Ok(game);
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(GameBrowseResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<GameBrowseResponseDto>> Index(string? searchTerm, decimal? minPrice, decimal? maxPrice, int? genreId, [FromQuery] List<int>? platformIds, SortBy sortBy = SortBy.Newest, int pageNumber = 1, int pageSize = 12)
        {
            _logger.LogInformation("Browsing games with searchTerm: {SearchTerm}, minPrice: {MinPrice}, maxPrice: {MaxPrice}, genreId: {GenreId}, platformIds: {PlatformIds}, sortBy: {SortBy}, pageNumber: {PageNumber}, pageSize: {PageSize}",
                searchTerm, minPrice, maxPrice, genreId, platformIds != null ? string.Join(",", platformIds) : "None", sortBy, pageNumber, pageSize);
            var (games, totalCount) = await _gameBrowseService.GetBrowseGamesAsync(pageNumber, pageSize, searchTerm, minPrice, maxPrice, genreId, platformIds, sortBy);
            var genres = await _gameBrowseService.GetGenreListAsync();
            var platforms = await _gameBrowseService.GetPlatformListAsync();
            var featuredGames = await _gameBrowseService.GetFeaturedGamesAsync();
            var responseDto = new GameBrowseResponseDto
            {
                Games = games,
                CurrentPage = pageNumber,
                TotalCount = totalCount,
                TotalPages = (totalCount + pageSize - 1) / pageSize,
                Genres = genres,
                Platforms = platforms,
                FeaturedGames = featuredGames.ToList(),
            };

            _logger.LogInformation("Returning {GameCount} games for browse request", games.Count);
            return Ok(responseDto);
        }
    }
}
