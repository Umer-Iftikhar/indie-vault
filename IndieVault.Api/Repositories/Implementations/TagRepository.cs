using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Data;
using IndieVault.Api.Models;

namespace IndieVault.Api.Repositories.Implementations
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(AppDbContext context) : base(context)
        {
        }
    }
}
