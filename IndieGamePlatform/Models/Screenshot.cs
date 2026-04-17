using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieGamePlatform.Models
{
    public class Screenshot
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public string ImagePath { get; set; } = String.Empty;
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
