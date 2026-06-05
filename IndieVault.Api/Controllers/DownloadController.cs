using IndieVault.Api.DTOs.Download.Responses;
using IndieVault.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadService _downloadService;
        private readonly ILogger<DownloadController> _logger;
        public DownloadController(IDownloadService downloadService, ILogger<DownloadController> logger)
        {
            _downloadService = downloadService;
            _logger = logger;
        }

        [HttpPost("{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Download(int gameId)
        {
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var downloadLink = string.Empty;

            try
            {
                _logger.LogInformation("Initiating game download for user: {UserId}, game: {GameId}", currentUser, gameId);
                downloadLink = await _downloadService.DownloadGameAsync(gameId, currentUser!);
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Download failed for user: {UserId}, game: {GameId} - Game not found", currentUser, gameId);

                return NotFound(new { message = "Game not found" });
            }
            
            return Ok(downloadLink);
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DownloadHistoryDto>>> MyDownloads()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var gameList = await _downloadService.GetDownloadHistoryAsync(currentUserId!);
            return Ok(gameList);
        }
    }
}
