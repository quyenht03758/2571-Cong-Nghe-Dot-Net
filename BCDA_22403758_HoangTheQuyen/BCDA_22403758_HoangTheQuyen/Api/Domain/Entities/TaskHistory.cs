namespace K8sManager.Api.Domain.Entities;

public class TaskHistory
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public int ClusterId { get; set; }
    public string TaskType { get; set; } = string.Empty; // Scale, Restart, ApplyYAML, Exec, Copy, Delete
    public string? Namespace { get; set; }
    public string? ResourceKind { get; set; }
    public string? ResourceName { get; set; }
    public string? Parameters { get; set; } // JSON
    public string Status { get; set; } = "Pending"; // Pending, Running, Success, Failed
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? Duration { get; set; } // milliseconds

    // Navigation
    public AppUser? User { get; set; }
    public ClusterConfig? Cluster { get; set; }
}

