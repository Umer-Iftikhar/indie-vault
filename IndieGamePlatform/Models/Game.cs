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
        [Required]
        [StringLength(50)]
        public string Engine { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Platforms { get; set; } = string.Empty;  
        [Required]
        [StringLength(500)]
        public string CoverImagePath { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string DownloadLink { get; set; } = string.Empty;
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Required]
        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        [Required]
        [ForeignKey("Developer")]
        public string DeveloperId { get; set; } = string.Empty;
        public Genre Genre { get; set; } = null!;
        public ApplicationUser Developer { get; set; } = null!;
    }
}
