using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    public interface IClusterRepository
    {
        Task<ClusterConfig?> GetByIdAsync(int id);
        Task<ClusterConfig?> GetDefaultAsync();
        Task<IEnumerable<ClusterConfig>> GetAllAsync();
        Task<int> CreateAsync(ClusterConfig cluster);
        Task<bool> UpdateAsync(ClusterConfig cluster);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetDefaultAsync(int id);
        
        // Alias for compatibility
        Task<int> AddAsync(ClusterConfig c);
    }
}

