using IndieVault.Api.DTOs;

namespace IndieVault.Api.Services.Interfaces.ExternalApis
{
    public interface IRawgApiService
    {
        Task<IEnumerable<RawgGameDto>> FetchGamesFromApiAsync(int count);
        Task<int> SynchronizeGamesFromApiAsync();
    }
}
