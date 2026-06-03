using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Api.Repositories.Implementations
{
    public class DownloadRepository : Repository<DownloadHistory>, IDownloadRepository
    {
        public DownloadRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<DownloadHistory>> GetDownloadHistoryByUserIdAsync(string userId)
        {
            var gameList = await _context.DownloadHistories // Access the DownloadHistories DbSet
               .Include(g => g.Game) // Include the related Game entity
               .Where(g => g.UserId == userId) // Filter by the specified userId
               .ToListAsync(); // Execute the query and return the results as a list
            return gameList;
        }
    }
}
