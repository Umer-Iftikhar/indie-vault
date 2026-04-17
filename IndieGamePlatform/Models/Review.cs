using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieGamePlatform.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } = 0;
        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
        [Required]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public Game Game { get; set; } = null!;

    }
}
