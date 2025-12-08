using System.Security.Cryptography;
using System.Text;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Services;

public class SettingsService
{
    private readonly IAppSettingRepository _settingRepo;
    private readonly Dictionary<string, string?> _cache = new();

    public SettingsService(IAppSettingRepository settingRepo)
    {
        _settingRepo = settingRepo;
    }

    /// <summary>
    /// Gets setting value by key with caching
    /// </summary>
    public async Task<string?> GetAsync(string key, string? defaultValue = null)
    {
        // Check cache first
        if (_cache.TryGetValue(key, out var cachedValue))
            return cachedValue ?? defaultValue;

        var setting = await _settingRepo.GetByKeyAsync(key).ConfigureAwait(false);
        var value = setting?.Value ?? defaultValue;

        // Decrypt if encrypted
        if (setting?.IsEncrypted == true && value != null)
        {
            value = Decrypt(value);
        }

        _cache[key] = value;
        return value;
    }

    /// <summary>
    /// Gets setting as integer
    /// </summary>
    public async Task<int> GetIntAsync(string key, int defaultValue = 0)
    {
        var value = await GetAsync(key).ConfigureAwait(false);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Gets setting as boolean
    /// </summary>
    public async Task<bool> GetBoolAsync(string key, bool defaultValue = false)
    {
        var value = await GetAsync(key).ConfigureAwait(false);
        return value?.ToLower() == "true" || value == "1" || defaultValue;
    }

    /// <summary>
    /// Sets setting value
    /// </summary>
    public async Task SetAsync(string key, string? value, string? category = null, string? description = null, bool isEncrypted = false, int? updatedBy = null)
    {
        var encryptedValue = isEncrypted && value != null ? Encrypt(value) : value;

        var setting = new AppSetting
        {
            Key = key,
            Value = encryptedValue,
            Category = category,
            Description = description,
            IsEncrypted = isEncrypted,
            UpdatedBy = updatedBy
        };

        await _settingRepo.UpsertAsync(setting).ConfigureAwait(false);

        // Update cache
        _cache[key] = value;
    }

    /// <summary>
    /// Gets all settings by category
    /// </summary>
    public async Task<IEnumerable<AppSetting>> GetByCategoryAsync(string category)
    {
        var settings = await _settingRepo.GetByCategoryAsync(category).ConfigureAwait(false);

        // Decrypt encrypted values
        foreach (var setting in settings)
        {
            if (setting.IsEncrypted && setting.Value != null)
            {
                setting.Value = Decrypt(setting.Value);
            }
        }

        return settings;
    }

    /// <summary>
    /// Gets all settings
    /// </summary>
    public async Task<IEnumerable<AppSetting>> GetAllAsync()
    {
        var settings = await _settingRepo.GetAllAsync().ConfigureAwait(false);

        // Decrypt encrypted values
        foreach (var setting in settings)
        {
            if (setting.IsEncrypted && setting.Value != null)
            {
                setting.Value = Decrypt(setting.Value);
            }
        }

        return settings;
    }

    /// <summary>
    /// Deletes a setting
    /// </summary>
    public async Task DeleteAsync(string key)
    {
        await _settingRepo.DeleteAsync(key).ConfigureAwait(false);
        _cache.Remove(key);
    }

    /// <summary>
    /// Clears the cache
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    // Simple encryption for demo - in production use proper encryption like AES with secure key management
    private static readonly byte[] _key = Encoding.UTF8.GetBytes("K8sManager2024!@"); // 16 bytes for AES-128
    private static readonly byte[] _iv = Encoding.UTF8.GetBytes("InitVector16Byte"); // 16 bytes IV

    private string Encrypt(string plainText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(plainText);
            sw.Flush();
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        catch
        {
            return plainText; // Return plain if encryption fails
        }
    }

    private string Decrypt(string cipherText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        catch
        {
            return cipherText; // Return cipher if decryption fails
        }
    }

    // Quick access methods for common settings
    public Task<string?> GetThemeAsync() => GetAsync("App.Theme", "Light");
    public Task SetThemeAsync(string theme, int? userId = null) => SetAsync("App.Theme", theme, "UI", updatedBy: userId);

    public Task<int> GetAutoRefreshIntervalAsync() => GetIntAsync("App.AutoRefreshInterval", 30);
    public Task<int> GetSessionTimeoutAsync() => GetIntAsync("Security.SessionTimeout", 480);
    public Task<int> GetMaxFailedLoginAttemptsAsync() => GetIntAsync("Security.MaxFailedLoginAttempts", 5);

    public Task<bool> GetCacheEnabledAsync() => GetBoolAsync("Cache.Enabled", true);
    public Task<int> GetCacheTTLAsync() => GetIntAsync("Cache.DefaultTTL", 300);

    public Task<string?> GetAIApiKeyAsync() => GetAsync("Integration.AIApiKey");
    public Task SetAIApiKeyAsync(string apiKey, int? userId = null) => SetAsync("Integration.AIApiKey", apiKey, "Integration", "AI API key", true, userId);
}
