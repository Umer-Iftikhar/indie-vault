using IndieVault.Api.DTOs;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IAdminStatisticsRepository
    {
        Task<MostWishlistedGameDto?> MostWishlistedGameAsync();
        Task<List<UserByRoleDto>> GetUsersByRoleAsync();
    }
}
