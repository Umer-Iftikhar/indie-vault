using IndieVault.Api.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IndieVault.Api.DTOs.Auth.Requests
{
    public class RegisterDto
    {
        [Required]
        [MinLength(2, ErrorMessage = "User name must be at least 2 characters long.")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long, must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public UserRole Role { get; set; }
    }
}
