namespace K8sManager.Api.Domain.Entities
{
    public class ClusterConfig
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string KubeconfigPath { get; set; } = "";
        public string ContextName { get; set; } = "";
        public bool IsDefault { get; set; }
        public string? Environment { get; set; } // Dev, Staging, Production
        public string? Description { get; set; }
        public int AddedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public AppUser? AddedByUser { get; set; }
    }
}

