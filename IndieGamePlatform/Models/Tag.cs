using System.ComponentModel.DataAnnotations;

namespace IndieGamePlatform.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public List<GameTag> GameTags { get; set; } = new List<GameTag>();
    }
}
