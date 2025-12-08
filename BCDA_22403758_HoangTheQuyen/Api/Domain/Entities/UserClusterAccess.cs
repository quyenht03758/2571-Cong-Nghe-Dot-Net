namespace K8sManager.Api.Domain.Entities;

public class UserClusterAccess
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ClusterId { get; set; }
    public bool CanRead { get; set; } = true;
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
    public int GrantedBy { get; set; }
    public DateTime GrantedAt { get; set; }

    // Navigation
    public AppUser? User { get; set; }
    public ClusterConfig? Cluster { get; set; }
    public AppUser? GrantedByUser { get; set; }
}

