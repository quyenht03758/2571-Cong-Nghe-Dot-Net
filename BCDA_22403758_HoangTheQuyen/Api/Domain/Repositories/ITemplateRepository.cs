using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories
{
    public interface ITemplateRepository
    {
        Task<Template?> GetByIdAsync(int id, bool includeVersions = false);
        Task<IEnumerable<Template>> GetAllAsync(string? category = null, string? search = null);
        Task<IEnumerable<(Template Template, TemplateVersion? CurrentVersion)>> GetAllWithCurrentVersionAsync(string? category = null, string? search = null);
        Task<int> CreateAsync(Template template);
        Task<bool> UpdateAsync(Template template);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<TemplateVersion>> GetVersionsByTemplateIdAsync(int templateId);
        Task<TemplateVersion?> GetCurrentVersionAsync(int templateId);
        Task<int> CreateVersionAsync(TemplateVersion version);
        Task SetCurrentVersionAsync(int templateId, int versionId);
        
        // Alias for compatibility
        Task<int> AddAsync(Template t);
    }
}

