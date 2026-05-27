using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.DTOs;

namespace IndieVault.Repositories.Interfaces
{
    public interface IAdminStatisticsRepository
    {
        Task<MostWishlistedGameDto?> MostWishlistedGameAsync();
        Task<List<UserByRoleDto>> GetUsersByRoleAsync();
    }
}
