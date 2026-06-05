using System.ComponentModel.DataAnnotations;

namespace IndieVault.Api.DTOs.Developer.Requests
{
    public class EditProfileDto
    {
        [StringLength(100)]
        public string GithubUserName { get; set; } = string.Empty;
    }
}
