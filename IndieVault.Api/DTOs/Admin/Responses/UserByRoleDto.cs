namespace IndieVault.Api.DTOs.Admin.Responses
{
    public class UserByRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
}
