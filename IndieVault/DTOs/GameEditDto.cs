namespace IndieVault.DTOs
{
    public class GameEditDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DownloadLink { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImagePath { get; set; } = string.Empty;
        public int SelectedGenreId { get; set; } 
        public int SelectedEngineId { get; set; } 
        public List<int> SelectedPlatformIds { get; set; } = new List<int>();
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        public GameFormDataDto FormData { get; set; } = new GameFormDataDto();
    }
}
