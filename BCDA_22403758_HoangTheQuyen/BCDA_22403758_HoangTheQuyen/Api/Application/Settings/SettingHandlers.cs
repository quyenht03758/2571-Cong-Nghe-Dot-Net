// Application/Settings/SettingHandlers.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Domain.Services;

namespace K8sManager.Api.Application.Settings;

// Commands
public record UpsertSettingCommand(string Key, string Value, string? Category, bool IsEncrypted, int? UpdatedBy);
public record DeleteSettingCommand(string Key);

// Queries
public record GetAllSettingsQuery();
public record GetSettingByKeyQuery(string Key);
public record GetSettingsByCategoryQuery(string Category);

// Handlers
public class UpsertSettingCommandHandler
{
    private readonly IAppSettingRepository _settingRepo;
    private readonly IEncryptionService _encryptionService;

    public UpsertSettingCommandHandler(
        IAppSettingRepository settingRepo,
        IEncryptionService encryptionService)
    {
        _settingRepo = settingRepo;
        _encryptionService = encryptionService;
    }

    public async Task<int> HandleAsync(UpsertSettingCommand command)
    {
        var value = command.IsEncrypted
            ? _encryptionService.Encrypt(command.Value)
            : command.Value;

        var existing = await _settingRepo.GetByKeyAsync(command.Key).ConfigureAwait(false);

        if (existing != null)
        {
            existing.Value = value;
            existing.Category = command.Category;
            existing.IsEncrypted = command.IsEncrypted;
            existing.UpdatedBy = command.UpdatedBy;
            existing.UpdatedAt = DateTime.UtcNow;
            await _settingRepo.UpdateAsync(existing).ConfigureAwait(false);
            return 1; // Return success
        }
        else
        {
            var newSetting = new AppSetting
            {
                Key = command.Key,
                Value = value,
                Category = command.Category,
                IsEncrypted = command.IsEncrypted,
                UpdatedBy = command.UpdatedBy,
                UpdatedAt = DateTime.UtcNow
            };
            return await _settingRepo.CreateAsync(newSetting).ConfigureAwait(false);
        }
    }
}

public class DeleteSettingCommandHandler
{
    private readonly IAppSettingRepository _settingRepo;

    public DeleteSettingCommandHandler(IAppSettingRepository settingRepo)
    {
        _settingRepo = settingRepo;
    }

    public async Task<bool> HandleAsync(DeleteSettingCommand command)
    {
        return await _settingRepo.DeleteAsync(command.Key).ConfigureAwait(false);
    }
}

public class GetAllSettingsQueryHandler
{
    private readonly IAppSettingRepository _settingRepo;
    private readonly IEncryptionService _encryptionService;

    public GetAllSettingsQueryHandler(
        IAppSettingRepository settingRepo,
        IEncryptionService encryptionService)
    {
        _settingRepo = settingRepo;
        _encryptionService = encryptionService;
    }

    public async Task<List<AppSetting>> HandleAsync(GetAllSettingsQuery query)
    {
        var settings = await _settingRepo.GetAllAsync().ConfigureAwait(false);
        return DecryptSettings(settings.ToList());
    }

    private List<AppSetting> DecryptSettings(List<AppSetting> settings)
    {
        return settings.Select(s =>
        {
            if (s.IsEncrypted)
            {
                s.Value = _encryptionService.Decrypt(s.Value);
            }
            return s;
        }).ToList();
    }
}

public class GetSettingByKeyQueryHandler
{
    private readonly IAppSettingRepository _settingRepo;
    private readonly IEncryptionService _encryptionService;

    public GetSettingByKeyQueryHandler(
        IAppSettingRepository settingRepo,
        IEncryptionService encryptionService)
    {
        _settingRepo = settingRepo;
        _encryptionService = encryptionService;
    }

    public async Task<AppSetting?> HandleAsync(GetSettingByKeyQuery query)
    {
        var setting = await _settingRepo.GetByKeyAsync(query.Key).ConfigureAwait(false);
        if (setting == null) return null;

        if (setting.IsEncrypted)
        {
            setting.Value = _encryptionService.Decrypt(setting.Value);
        }
        return setting;
    }
}

public class GetSettingsByCategoryQueryHandler
{
    private readonly IAppSettingRepository _settingRepo;
    private readonly IEncryptionService _encryptionService;

    public GetSettingsByCategoryQueryHandler(
        IAppSettingRepository settingRepo,
        IEncryptionService encryptionService)
    {
        _settingRepo = settingRepo;
        _encryptionService = encryptionService;
    }

    public async Task<List<AppSetting>> HandleAsync(GetSettingsByCategoryQuery query)
    {
        var settings = await _settingRepo.GetByCategoryAsync(query.Category).ConfigureAwait(false);
        return settings.Select(s =>
        {
            if (s.IsEncrypted)
            {
                s.Value = _encryptionService.Decrypt(s.Value);
            }
            return s;
        }).ToList();
    }
}
