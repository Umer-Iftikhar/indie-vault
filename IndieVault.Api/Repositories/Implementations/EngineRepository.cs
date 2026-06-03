using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace IndieVault.Api.Repositories.Implementations
{
    public class EngineRepository : Repository<Engine>, IEngineRepository
    {
        public EngineRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Engine?> GetEngineByNameAsync(string name)
        {
            return await _context.Engines.FirstOrDefaultAsync(e => e.Name == name);
        }
    }
}
