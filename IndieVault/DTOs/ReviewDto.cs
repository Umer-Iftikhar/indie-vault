namespace IndieVault.DTOs
{
    public class ReviewDto
    {
        public string ReviewerName { get; set;} = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
    }
}
