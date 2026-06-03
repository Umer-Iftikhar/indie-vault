using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Interfaces
{
    public interface IEngineRepository : IRepository<Engine>
    {
        Task<Engine?> GetEngineByNameAsync(string name);
    }
}
