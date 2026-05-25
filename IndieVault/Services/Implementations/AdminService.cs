using IndieVault.DTOs;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class AdminService : IAdminService
    {
        public async Task<AdminDashboardDto> GetAdminDashboardDataAsync()
        {
            throw new NotImplementedException();
        }
        public async Task AdminCreateGenreAsync(string genreName)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> GenreExistsAsync(string genreName)
        {
            throw new NotImplementedException();
        }
        public async Task AdminDeleteGenreAsync(int genreId)
        {
            throw new NotImplementedException();
        }
        public async Task IsGameFeatureAsync(int gameId)
        {
            throw new NotImplementedException();
        }
    }
}
