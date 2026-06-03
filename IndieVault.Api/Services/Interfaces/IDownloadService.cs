using IndieVault.Api.DTOs;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IDownloadService
    {
        Task<string> DownloadGameAsync(int gameId, string userId);
        Task<List<DownloadHistoryDto>> GetDownloadHistoryAsync(string userId);
    }
}
