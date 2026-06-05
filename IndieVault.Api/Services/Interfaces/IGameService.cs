using IndieVault.Api.DTOs.Game.Requests;
using IndieVault.Api.DTOs.Game.Responses;
using IndieVault.Api.DTOs.Rawg.External;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IGameService
    {
        Task<GameFormDataDto> GetFormDataAsync();
        Task<int> UploadGameAsync(GameUploadDto uploadDto, string userId);
        Task<List<MyGameDto>> GetMyGamesAsync(string userId);
        Task DeleteGameAsync(int gameId, string userId);
        Task<GameEditDto> GetGameForEditAsync(int gameId, string userId);
        Task UpdateGameAsync(GameUpdateDto updateDto, string userId, int gameId);
        Task<GameDetailDto> GetGameDetailsAsync(int gameId, string userId);

        // method to get count of all the games of a particular developer (account controller)
        Task<int> GetDevGameCountAsync(string userId);

        // method to check if a game exists by title or external ID (RawgApiService)
        Task<bool> CreateGameFromApiAsync(RawgGameUploadDto dto);
    }
}
