using System.ComponentModel.DataAnnotations;

namespace IndieGamePlatform.Models
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        [StringLength(100)]
        public string GithubUserName { get; set; } = string.Empty;  
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
