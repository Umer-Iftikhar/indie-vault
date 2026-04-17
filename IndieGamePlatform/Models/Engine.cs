using System.ComponentModel.DataAnnotations;

namespace IndieGamePlatform.Models
{
    public class Engine
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new List<Game>();
    }
}
