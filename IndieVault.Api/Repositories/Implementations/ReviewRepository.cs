using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Api.Repositories.Implementations
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> ReviewExistsAsync(string userId, int gameId)
        {
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.GameId == gameId);
        }
        public async Task<int> CountReviewsAsync()
        {
            return await _context.Reviews.CountAsync();
        }
    }
}
