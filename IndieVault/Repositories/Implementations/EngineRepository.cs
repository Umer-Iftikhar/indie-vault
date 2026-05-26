using IndieVault.Data;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;

namespace IndieVault.Repositories.Implementations
{
    public class EngineRepository : Repository<Engine>, IEngineRepository
    {
        public EngineRepository(AppDbContext context) : base(context)
        {
        }
    }
}
