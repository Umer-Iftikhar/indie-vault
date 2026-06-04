namespace IndieVault.Api.DTOs.Review.Requests
{
    public class CreateReviewDto
    {
        public int GameId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
