namespace K8sManager.Api.Domain.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClusterId { get; set; }
        public string? Namespace { get; set; }
        public string? ResourceKind { get; set; } // Pod, Deployment, Service
        public string? ResourceName { get; set; }
        public string DisplayName { get; set; } = "";
        public string? Notes { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public AppUser? User { get; set; }
        public ClusterConfig? Cluster { get; set; }
    }
}

