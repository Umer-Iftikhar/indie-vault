using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieVault.Models
{
    public class Review : BaseEntity
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } = 0;
        
        [StringLength(1000)]
        public string? Comment { get; set; } 
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
