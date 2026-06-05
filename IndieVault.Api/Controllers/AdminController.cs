using IndieVault.Api.DTOs.Admin.Requests;
using IndieVault.Api.DTOs.Admin.Responses;
using IndieVault.Api.Services.Interfaces;
using IndieVault.Api.Services.Interfaces.ExternalApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IRawgApiService _rawgApiService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, IRawgApiService rawgApiService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _rawgApiService = rawgApiService;
            _logger = logger;
        }


        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AdminDashboardDto>> Dashboard()
        {
            var dashboardDto = await _adminService.GetAdminDashboardDataAsync(); // Replace with actual method to get dashboard data

            return Ok(dashboardDto);
        }

        
        [HttpPost("genres")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
        {
            var genre = new GenreDto();
            try
            {
                genre = await _adminService.AdminCreateGenreAsync(createGenreDto.GenreName);

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error creating genre: {GenreName}", createGenreDto.GenreName);
                return BadRequest();
            }

            return Created($"api/admin/genres/{genre.GenreId}", genre);
        }
        
        [HttpDelete("genres/{genreId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GenreDelete(int genreId)
        {
            try
            {
                await _adminService.AdminDeleteGenreAsync(genreId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error deleting genre with Id: {GenreId}", genreId);
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPatch("games/{gameId}/feature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ToggleFeature(int gameId)
        {
            await _adminService.IsGameFeatureAsync(gameId);

            return Ok(new {id = gameId, message = "Feature status toggled"});
        }

        [HttpPost("sync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SyncGames()
        {
            try
            {
                int syncedCount = await _rawgApiService.SynchronizeGamesFromApiAsync();

                _logger.LogInformation("RAWG sync completed successfully. Total games synced: {SyncedCount}", syncedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RAWG sync failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Game synchronization failed. Please try again later." });
            }
            return Ok(new { message = "Game synchronization completed."});
        }
    }
}
