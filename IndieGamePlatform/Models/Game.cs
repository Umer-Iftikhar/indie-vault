using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieGamePlatform.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        // --- Engine (3NF Foreign Key) ---
        [Required]
        [ForeignKey(nameof(Engine))]
        public int EngineId { get; set; }
        public Engine Engine { get; set; } = null!;

        // --- Platforms (3NF Many-to-Many) ---
        public List<GamePlatform> GamePlatforms { get; set; } = new();

        [Required]
        [StringLength(500)]
        public string CoverImagePath { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string DownloadLink { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // --- Relationships ---
        [Required]
        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Developer))]
        public string DeveloperId { get; set; } = string.Empty;
        public ApplicationUser Developer { get; set; } = null!;

        // Navigation for other related data
        public List<GameTag> GameTags { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}