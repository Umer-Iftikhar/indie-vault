using System.ComponentModel.DataAnnotations;

namespace IndieGamePlatform.Models
{
    public class GameTag
    {
        public int GameId { get; set; }
        public int TagId { get; set; }
        public Game Game { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
