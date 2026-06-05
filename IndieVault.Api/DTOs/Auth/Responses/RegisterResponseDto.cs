using IndieVault.Api.Enums;

namespace IndieVault.Api.DTOs.Auth.Responses
{
    public class RegisterResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
