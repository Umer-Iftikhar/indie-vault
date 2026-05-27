using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }
        
        public async Task AddToWishlistAsync(int gameId, string userId)
        {
            // Check if the user has already wishlisted the game
            if (await _wishlistRepository.WishlistExistsAsync(userId, gameId))
            {
                throw new InvalidOperationException("Game is already in the wishlist.");
            }

            // Create a new wishlist entry
            var wishlistItem = new Wishlist
            {
                GameId = gameId,
                UserId = userId
            };

            // Save the wishlist entry to the database
            await _wishlistRepository.CreateAsync(wishlistItem);
        }
        public async Task RemoveFromWishlistAsync(int gameId, string userId)
        {
            // Check if the wishlist entry exists
            var wishlistItem = await _wishlistRepository.GetWishlistEntryAsync(userId, gameId);

            if (wishlistItem == null)
            {
                throw new InvalidOperationException("Wishlist entry not found.");
            }

            // Delete the wishlist entry from the database
            await _wishlistRepository.DeleteAsync(wishlistItem.Id);
        }
        public async Task<List<WishlistItemDto>> GetUserWishlistAsync(string userId)
        {
            // Retrieve the wishlist items for the user
            var wishlistGames = await _wishlistRepository.GetWishlistByUserIdAsync(userId);

            // Map the wishlist items to WishlistItemDto and return the list
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
