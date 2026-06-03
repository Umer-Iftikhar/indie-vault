namespace IndieVault.Api.DTOs
{
    public class DownloadHistoryDto
    {
        public string GameName { get; set; } = string.Empty;
        public DateTime DownloadTime { get; set; } = DateTime.UtcNow;
        public int GameId { get; set; }
    }
}
