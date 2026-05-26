using IndieVault.Data;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;

namespace IndieVault.Repositories.Implementations
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(AppDbContext context) : base(context)
        {
        }
    }
}
