using K8sManager.Api.Domain.Entities;

namespace K8sManager.Services.Deprecated
{
    // Authentication & Session interfaces
    internal interface ISessionService
    {
        Task<(bool success, string? token, AppUser? user)> LoginAsync(string username, string password);
        Task LogoutAsync(string token);
        Task<bool> ValidateSessionAsync(string token);
        Task<AppUser?> GetUserByTokenAsync(string token);
        AppUser? CurrentUser { get; }
    }

    internal interface IAuthService
    {
        Task<(bool success, string message, AppUser? user)> RegisterAsync(string username, string password, string role = "Viewer");
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> UpdateRoleAsync(int userId, string newRole);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    // Audit logging interface
    internal interface IAuditService
    {
        Task LogAsync(int userId, string action, string resourceType, string resourceName, string ns, bool success, string? errorMessage);
        Task<IEnumerable<AuditLog>> GetLogsAsync(DateTime? from = null, DateTime? to = null, int? userId = null);
    }

    // Template management interface
    internal interface ITemplateService
    {
        Task<IEnumerable<(int Id, string Name, string Category)>> ListTemplatesAsync();
        Task<(bool success, int templateId)> CreateTemplateAsync(string name, string category, string yamlContent, string description, int createdBy);
        Task<bool> UpdateTemplateAsync(int id, string name, string category, string yamlContent, string description);
        Task<bool> DeleteTemplateAsync(int id);
        Task<string?> GetTemplateYamlAsync(int id);
        Task<IEnumerable<(int Id, int Version, string YamlContent, DateTime CreatedAt)>> GetTemplateVersionsAsync(int templateId);
    }

    // Settings management interface
    internal interface ISettingsService
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, bool isEncrypted = false);
        Task<Dictionary<string, string>> GetAllAsync();
        Task DeleteAsync(string key);
    }

    // Favorites management interface (if you want to add it)
    internal interface IFavoriteService
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
        Task<bool> AddFavoriteAsync(int userId, string resourceType, string resourceName, string ns, string cluster, string? notes);
        Task<bool> RemoveFavoriteAsync(int favoriteId);
        Task<bool> UpdateNotesAsync(int favoriteId, string notes);
    }
}
