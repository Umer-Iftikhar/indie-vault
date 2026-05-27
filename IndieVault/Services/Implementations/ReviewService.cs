using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IGameRepository _gameRepository;
        public ReviewService(IReviewRepository reviewRepository, IGameRepository gameRepository)
        {
            _reviewRepository = reviewRepository;
            _gameRepository = gameRepository;
        }

        public async Task<bool> HasUserReviewedAsync(int gameId, string userId)
        {
            return await _reviewRepository.ReviewExistsAsync(userId, gameId);
        }
        public async Task GetReviewFormAsync(int id)
        {
            bool gameExists = await _gameRepository.ExistsAsync(id);
            if (!gameExists)
            {
                throw new KeyNotFoundException("Game not found.");
            }
        }
        public async Task CreateReviewAsync(CreateReviewDto createReviewDto, string userId)
        {
            var review = new Review
            {
                GameId = createReviewDto.GameId,
                UserId = userId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                ReviewDate = DateTime.UtcNow
            };
            await _reviewRepository.CreateAsync(review);
        }
        public async Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            if (review.UserId != userId && !isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this review.");
            }
            await _reviewRepository.DeleteAsync(reviewId);
        }
    }
}
