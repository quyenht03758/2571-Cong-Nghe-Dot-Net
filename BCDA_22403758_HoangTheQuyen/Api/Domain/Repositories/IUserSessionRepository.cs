using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByIdAsync(long id);
    Task<UserSession?> GetByTokenAsync(string sessionToken);
    Task<IEnumerable<UserSession>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserSession>> GetActiveSessionsAsync(int? userId = null);
    Task<long> CreateAsync(UserSession session);
    Task<bool> DeleteByTokenAsync(string sessionToken);
    Task<int> DeleteAsync(long id);
    Task<int> DeleteByUserIdAsync(int userId);
    Task DeleteExpiredAsync();
}

