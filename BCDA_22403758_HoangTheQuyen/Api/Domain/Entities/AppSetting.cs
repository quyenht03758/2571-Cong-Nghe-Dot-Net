namespace K8sManager.Api.Domain.Entities;

public class AppSetting
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Category { get; set; } // General, Security, UI, Integration
    public string? Description { get; set; }
    public bool IsEncrypted { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public AppUser? UpdatedByUser { get; set; }
}

