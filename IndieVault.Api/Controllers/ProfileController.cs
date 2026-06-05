using IndieVault.Api.DTOs.Developer.Requests;
using IndieVault.Api.DTOs.Developer.Responses;
using IndieVault.Api.DTOs.GitHub.Responses;
using IndieVault.Api.Models;
using IndieVault.Api.Services.Interfaces;
using IndieVault.Api.Services.Interfaces.ExternalApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGitHubService _gitHubService;
        private readonly IGameService _gameService;

        public ProfileController(UserManager<ApplicationUser> userManager, IGitHubService gitHubService, IGameService gameService)
        {
            _userManager = userManager;
            _gitHubService = gitHubService;
            _gameService = gameService;
        }

        [Authorize]
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(DevProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DevProfileDto>> Profile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }


            GitHubProfileDto? git = null;
            if (user.GithubUserName != null)
            {
                git = await _gitHubService.GetProfileAsync(user.GithubUserName);
            }

            var totalGames = await _gameService.GetDevGameCountAsync(userId);

            var profileDto = new DevProfileDto
            {
                UserId = userId,
                TotalGames = totalGames,
                Email = user.Email,
                Name = user.UserName,
                JoinDate = user.CreatedDate,
                GitHubProfile = git
            };
            return Ok(profileDto);
        }

        [Authorize(Roles = "GameDev")]
        [HttpPatch("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> EditProfile(string userId, EditProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }


            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUserId != userId)
            {
                return Forbid();
            }

            // Update the domain model from the Dto
            user.GithubUserName = dto.GithubUserName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Profile updated successfully" });
            }


            return BadRequest(new { message = "Failed to update profile" });
        }
    }
}
