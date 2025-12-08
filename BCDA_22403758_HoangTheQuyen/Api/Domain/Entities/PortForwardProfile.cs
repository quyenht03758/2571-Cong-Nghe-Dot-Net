namespace K8sManager.Api.Domain.Entities;

public class PortForwardProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClusterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? PodName { get; set; }
    public string? PodSelector { get; set; }
    public int LocalPort { get; set; }
    public int RemotePort { get; set; }
    public string? Description { get; set; }
    public bool AutoStart { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public AppUser? User { get; set; }
    public ClusterConfig? Cluster { get; set; }
}

public class ActivePortForward
{
    public int Id { get; set; }
    public int? ProfileId { get; set; }
    public int UserId { get; set; }
    public int ClusterId { get; set; }
    public string Namespace { get; set; } = string.Empty;
    public string PodName { get; set; } = string.Empty;
    public int LocalPort { get; set; }
    public int RemotePort { get; set; }
    public int? ProcessId { get; set; }
    public string Status { get; set; } = "Running"; // Running, Stopped, Failed
    public DateTime StartedAt { get; set; }
    public DateTime? StoppedAt { get; set; }

    // Navigation
    public PortForwardProfile? Profile { get; set; }
    public AppUser? User { get; set; }
    public ClusterConfig? Cluster { get; set; }
}

