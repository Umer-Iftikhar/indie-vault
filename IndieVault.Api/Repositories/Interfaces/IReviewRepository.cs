using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<bool> ReviewExistsAsync(string userId, int gameId);
        Task<int> CountReviewsAsync();
    }
}
