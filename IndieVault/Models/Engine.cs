using System.ComponentModel.DataAnnotations;

namespace IndieVault.Models
{
    public class Engine : BaseEntity
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<Game> Games { get; set; } = new List<Game>();
    }
}
