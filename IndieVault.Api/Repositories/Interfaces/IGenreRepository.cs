using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<bool> GenreExistsByNameAsync(string name);
        Task<Genre?> GetGenreByNameAsync(string name);
    }
}
