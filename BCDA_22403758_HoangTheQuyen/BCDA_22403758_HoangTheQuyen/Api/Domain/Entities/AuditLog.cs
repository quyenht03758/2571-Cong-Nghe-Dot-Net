namespace K8sManager.Api.Domain.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int? ClusterId { get; set; }
        public string Action { get; set; } = ""; // View, Create, Update, Delete, Scale, Restart, Shell, PortForward
        public string? ResourceKind { get; set; }
        public string? ResourceName { get; set; }
        public string? Namespace { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RequestPayload { get; set; } // JSON/YAML
        public string? ResponseData { get; set; }
        public string? IpAddress { get; set; }
        public int? Duration { get; set; } // milliseconds
        public DateTime CreatedAt { get; set; }

        // Navigation
        public AppUser? User { get; set; }
        public ClusterConfig? Cluster { get; set; }
    }
}

