using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<bool> WishlistExistsAsync(string userId, int gameId);
        Task<List<Wishlist>> GetWishlistByUserIdAsync(string userId);
        Task<Wishlist?> GetWishlistEntryAsync(string userId, int gameId);
    }
}
