using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetByIdAsync(int id);
        Task<IEnumerable<Favorite>> GetByUserIdAsync(int userId);
        Task<int> CreateAsync(Favorite favorite);
        Task<bool> UpdateAsync(Favorite favorite);
        Task<bool> DeleteAsync(int id);
        
        // Aliases for compatibility
        Task<int> AddAsync(Favorite f);
        Task<IEnumerable<Favorite>> ListByUserAsync(int userId);
    }
}

