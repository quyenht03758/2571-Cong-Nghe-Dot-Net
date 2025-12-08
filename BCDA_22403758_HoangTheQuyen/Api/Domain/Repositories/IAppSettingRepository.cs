using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories;

/// <summary>
/// Repository for global application settings
/// Key is the primary key (not auto-increment)
/// </summary>
public interface IAppSettingRepository
{
    Task<AppSetting?> GetByKeyAsync(string key);
    Task<IEnumerable<AppSetting>> GetAllAsync();
    Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category);
    Task<bool> UpsertAsync(AppSetting setting); // INSERT or UPDATE based on Key
    Task<bool> DeleteAsync(string key);
    Task<bool> UpdateAsync(AppSetting setting) => UpsertAsync(setting);
    Task<int> CreateAsync(AppSetting setting);
}

