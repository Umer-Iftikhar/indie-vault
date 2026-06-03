using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IScreenshotRepository : IRepository<Screenshot>
    {
        Task AddScreenshotsAsync(List<Screenshot> screenshots);
        Task<List<Screenshot>> GetScreenshotsByGameIdAsync(int gameId);
        Task DeleteScreenshotsByGameIdAsync(int gameId);
    }
}
