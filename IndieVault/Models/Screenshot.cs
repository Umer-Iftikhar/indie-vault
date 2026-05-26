using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieVault.Models
{
    public class Screenshot : BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string ImagePath { get; set; } = String.Empty;
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}
