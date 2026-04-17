using System.ComponentModel.DataAnnotations;

namespace IndieGamePlatform.Models
{
    public class Platform
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public List<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    }
}
