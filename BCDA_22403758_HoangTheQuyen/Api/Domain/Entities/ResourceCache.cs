namespace K8sManager.Api.Domain.Entities;

public class ResourceCache
{
    public long Id { get; set; }
    public int ClusterId { get; set; }
    public string? Namespace { get; set; }
    public string ResourceKind { get; set; } = string.Empty;
    public string CacheData { get; set; } = string.Empty; // JSON snapshot
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public ClusterConfig? Cluster { get; set; }
}

public class Notification
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Info, Warning, Error, Success
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation
    public AppUser? User { get; set; }
}

