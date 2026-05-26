using IndieVault.Models;

namespace IndieVault.Repositories.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<bool> WishlistExistsAsync(string userId, int gameId);
    }
}
