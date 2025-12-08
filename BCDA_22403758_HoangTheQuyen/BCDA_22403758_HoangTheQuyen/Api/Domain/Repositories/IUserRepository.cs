// Api/Domain/Repositories/IUserRepository.cs
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories;

/// <summary>
/// Repository interface for User aggregate (Domain layer)
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<(IEnumerable<User> Items, int TotalCount)> GetUsersAsync(int page, int pageSize) => GetPagedAsync(page, pageSize);
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string username);
    Task<User?> FindByIdAsync(int id) => GetByIdAsync(id);
    Task<User?> FindByUsernameAsync(string username) => GetByUsernameAsync(username);
}

