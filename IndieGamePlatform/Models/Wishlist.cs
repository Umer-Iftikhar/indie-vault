using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndieGamePlatform.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
