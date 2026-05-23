using IndieVault.DTOs;
using IndieVault.ViewModels;

namespace IndieVault.Services.Interfaces
{
    public interface IGameService
    {
        Task<GameFormDataDto> GetFormDataAsync();
        Task UploadGameAsync(GameUploadViewModel model, string userId);
        Task<List<MyGameDto>> GetMyGamesAsync(string userId);
        Task DeleteGameAsync(int gameId, string userId);
        Task<GameEditDto> GetGameForEditAsync(int gameId, string userId);
        Task UpdateGameAsync(GameEditViewModel model, string userId);
        Task<GameDetailDto> GetGameDetailsAsync(int gameId, string userId);
    }
}
