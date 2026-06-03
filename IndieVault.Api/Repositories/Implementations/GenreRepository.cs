using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Api.Repositories.Implementations
{
    public class GenreRepository : Repository<Genre>, IGenreRepository 
    {
        public GenreRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> GenreExistsByNameAsync(string name)
        {
            return await _context.Genres.AnyAsync(g => g.Name.ToLower() == name.ToLower());
        }
        
        public async Task<Genre?> GetGenreByNameAsync(string name)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Name == name);
        }
    }
}
