using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasUserReviewedAsync(int gameId, string userId)
        {
            return await _context.Reviews.AnyAsync(r => r.GameId == gameId && r.UserId == userId);
        }
        public async Task GetReviewFormAsync(int id)
        {
            bool gameExists = await _context.Games.AnyAsync(game => game.Id == id);
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
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found.");
            }
            if (review.UserId != userId && !isAdmin)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this review.");
            }
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
