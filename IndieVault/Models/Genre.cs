using Bogus.DataSets;
using System.ComponentModel.DataAnnotations;


namespace IndieVault.Models
{
    public class Genre : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
