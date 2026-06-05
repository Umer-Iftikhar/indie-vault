using IndieVault.Api.DTOs.Auth.Requests;
using IndieVault.Api.DTOs.Auth.Responses;
using IndieVault.Api.Models;
using IndieVault.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IndieVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
        }
        
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto registerDto)
        {
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                CreatedDate = DateTime.UtcNow,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }

            var userRole = await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());
            if (!userRole.Succeeded)
            {
                return BadRequest("Failed to assign role to user");
            }

            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                UserRole = registerDto.Role,
                Message = "User registered successfully"
            };

            return Created($"api/profile/{user.Id}", response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid Email or Password");
            }
            var password = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!password)
            {
                return Unauthorized("Invalid Email or Password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var authResponse = _tokenService.GenerateToken(user, roles);
            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id);
            authResponse.RefreshToken = refreshToken;

            return Ok(authResponse);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto refreshRequestDto)
        {
            var validateRefreshToken = await _refreshTokenService.ValidateRefreshToken(refreshRequestDto.RefreshToken);

            if (validateRefreshToken == null)
            {
                return Unauthorized("Invalid Refresh Token");
            }

            var user = await _userManager.FindByIdAsync(validateRefreshToken.UserId);
            if (user == null)
            {
                return Unauthorized("Invalid Refresh Token");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var authResponse = _tokenService.GenerateToken(user, roles);

            await _refreshTokenService.RevokeRefreshToken(validateRefreshToken);

            var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id);

            authResponse.RefreshToken = refreshToken;

            return Ok(authResponse);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Logout(RefreshRequestDto refreshRequestDto)
        {
            var refreshToken = await _refreshTokenService.ValidateRefreshToken(refreshRequestDto.RefreshToken);
            if (refreshToken == null)
            {
                return BadRequest("Invalid Refresh Token");
            }
            await _refreshTokenService.RevokeRefreshToken(refreshToken);
            return NoContent();
        }
     }
}