using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IDownloadRepository : IRepository<DownloadHistory>
    {
        Task<List<DownloadHistory>> GetDownloadHistoryByUserIdAsync(string userId);
    }
}
