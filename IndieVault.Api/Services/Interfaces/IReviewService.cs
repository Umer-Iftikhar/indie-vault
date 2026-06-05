using IndieVault.Api.DTOs.Review.Requests;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IReviewService
    {
        Task GetReviewFormAsync(int id);
        Task<int> CreateReviewAsync(CreateReviewDto createReviewDto, string userId, int gameId);
        Task<bool> HasUserReviewedAsync(int gameId, string userId);
        Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin);
    }
}
