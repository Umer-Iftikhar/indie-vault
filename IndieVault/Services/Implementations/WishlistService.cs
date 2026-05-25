using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context; 
        public WishlistService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> HasUserWishlistedAsync(int gameId, string userId)
        {
            return await _context.Wishlists.AnyAsync(w => w.GameId == gameId && w.UserId == userId);
        }
        public async Task AddToWishlistAsync(int gameId, string userId)
        {
            var wishlistItem = new Wishlist
            {
                GameId = gameId,
                UserId = userId
            };
           
            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveFromWishlistAsync(int gameId, string userId)
        {
            var wishlistEntry = await _context.Wishlists.FirstOrDefaultAsync(w => w.GameId == gameId && w.UserId == userId);

            if (wishlistEntry == null)
            {
                throw new InvalidOperationException("Wishlist entry not found.");
            }
             _context.Wishlists.Remove(wishlistEntry!);
            await _context.SaveChangesAsync();
        }
        public async Task<List<WishlistItemDto>> GetUserWishlistAsync(string userId)
        {
            var wishlistGames = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Game)
                    .ThenInclude(g => g.Genre)
                .ToListAsync();
            return wishlistGames.Select(w => new WishlistItemDto
            {
                GameId = w.GameId,
                CoverImagePath = w.Game.CoverImagePath,
                CreatedDate = w.CreatedDate,
                GameGenre = w.Game.Genre.Name,
                Price = w.Game.Price,
                Title = w.Game.Title
            }).ToList();
        }
    }
}
