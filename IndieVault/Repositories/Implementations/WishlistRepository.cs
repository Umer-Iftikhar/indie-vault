using IndieVault.Data;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Repositories.Implementations
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> WishlistExistsAsync(string userId, int gameId)
        {
            return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.GameId == gameId);
        }
    }
}
