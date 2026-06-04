using IndieVault.Api.DTOs.Wishlist.Responses;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IWishlistService
    {
        Task AddToWishlistAsync(int gameId, string userId);
        Task RemoveFromWishlistAsync(int gameId, string userId);
        Task<List<WishlistItemDto>> GetUserWishlistAsync(string userId);
    }
}
