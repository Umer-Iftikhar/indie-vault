using IndieVault.Api.DTOs.Admin.Responses;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IAdminStatisticsRepository
    {
        Task<MostWishlistedGameDto?> MostWishlistedGameAsync();
        Task<List<UserByRoleDto>> GetUsersByRoleAsync();
    }
}
