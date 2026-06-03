using IndieVault.Api.DTOs;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetAdminDashboardDataAsync();
        Task AdminCreateGenreAsync(string genreName);
        Task<bool> GenreExistsAsync(string genreName);
        Task AdminDeleteGenreAsync(int genreId);
        Task IsGameFeatureAsync(int gameId);
        Task<LookupDto> GetGenreByIdAsync(int genreId);
    }
}
