namespace K8sManager.Api.Domain.Entities;

public class TemplateVersion
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string YamlContent { get; set; } = string.Empty;
    public string? ChangeLog { get; set; }
    public bool IsCurrent { get; set; } = true;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Template? Template { get; set; }
    public AppUser? Creator { get; set; }
}

