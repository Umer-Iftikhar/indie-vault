using IndieVault.Models;

namespace IndieVault.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        public Task<T?> GetByIdAsync(int id);
        public Task<bool> ExistsAsync(int id);
        public Task UpdateAsync(T entity);
        public Task<T> CreateAsync(T entity);
        public Task DeleteAsync(int id);
        public Task<List<T>> GetAllAsync();
    }
}
