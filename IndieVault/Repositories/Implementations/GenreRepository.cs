using IndieVault.Data;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;

namespace IndieVault.Repositories.Implementations
{
    public class GenreRepository : Repository<Genre>, IGenreRepository 
    {
        public GenreRepository(AppDbContext context) : base(context)
        {
        }
    }
}
