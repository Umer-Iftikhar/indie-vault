using IndieVault.Api.Services.Interfaces;
using IndieVault.Api.Models;
using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.DTOs.Review.Requests;

namespace IndieVault.Api.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<ReviewService> _logger;
        public ReviewService(IReviewRepository reviewRepository, IGameRepository gameRepository, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task<bool> HasUserReviewedAsync(int gameId, string userId)
        {
            _logger.LogInformation("Checking if user {UserId} has reviewed game {GameId}", userId, gameId);

            var result = await _reviewRepository.ReviewExistsAsync(userId, gameId);

            _logger.LogInformation("User {UserId} review exists for game {GameId}: {Exists}", userId, gameId, result);

            return result;
        }
        public async Task GetReviewFormAsync(int id)
        {
            _logger.LogInformation("Validating game existence for review form, GameId: {GameId}", id);

            bool gameExists = await _gameRepository.ExistsAsync(id);
            if (!gameExists)
            {
                _logger.LogWarning("Review form requested for non-existent game {GameId}", id);
                throw new KeyNotFoundException("Game not found.");
            }
            _logger.LogInformation("Game {GameId} exists, returning review form", id);
        }
        public async Task<int> CreateReviewAsync(CreateReviewDto createReviewDto, string userId, int gameId)
        {
            _logger.LogInformation("User {UserId} creating review for game {GameId} with rating {Rating}", userId, gameId, createReviewDto.Rating);

            var review = new Review
            {
                GameId = gameId,
                UserId = userId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                ReviewDate = DateTime.UtcNow
            };

            try
            {
                await _reviewRepository.CreateAsync(review);
                _logger.LogInformation("Review created successfully for game {GameId} by user {UserId}", gameId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create review for game {GameId} by user {UserId}", gameId, userId);
                throw; // Re-throw to let caller handle
            }
            return review.Id;
        }
        public async Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin)
        {
            _logger.LogInformation("User {UserId} (IsAdmin: {IsAdmin}) attempting to delete review {ReviewId}", userId, isAdmin, reviewId);
            
            var review = await _reviewRepository.GetByIdAsync(reviewId);


            if (review == null)
            {
                _logger.LogWarning("Delete failed: Review {ReviewId} not found", reviewId);
                throw new KeyNotFoundException("Review not found.");
            }
            if (review.UserId != userId && !isAdmin)
            {
                _logger.LogWarning("Unauthorized delete attempt on review {ReviewId} by user {UserId} (Owner: {OwnerId})", reviewId, userId, review.UserId);
                throw new UnauthorizedAccessException("You do not have permission to delete this review.");
            }
            await _reviewRepository.DeleteAsync(reviewId);
            _logger.LogInformation("Review {ReviewId} deleted successfully by user {UserId}", reviewId, userId); 
        }
    }
}
