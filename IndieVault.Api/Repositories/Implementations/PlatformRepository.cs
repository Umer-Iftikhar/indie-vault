using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Implementations
{
    public class PlatformRepository : Repository<Platform>, IPlatformRepository
    {
        public PlatformRepository(AppDbContext context) : base(context)
        {
        }
    }
}
