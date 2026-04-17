using Bogus.DataSets;
using System.ComponentModel.DataAnnotations;


namespace IndieGamePlatform.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
