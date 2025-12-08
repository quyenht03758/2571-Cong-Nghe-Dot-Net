// Api/DTOs/SettingsDTOs.cs
namespace K8sManager.Api.DTOs;

public class UpsertSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsEncrypted { get; set; }
}

public record SettingRequest(
    string Key,
    string Value,
    string Category,
    bool IsEncrypted = false,
    string? Description = null
);

public record SettingResponse(
    int Id,
    string Key,
    string Value,
    string Category,
    bool IsEncrypted,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record SettingFilter(
    string? Category = null,
    string? SearchTerm = null,
    bool? IsEncrypted = null
);

