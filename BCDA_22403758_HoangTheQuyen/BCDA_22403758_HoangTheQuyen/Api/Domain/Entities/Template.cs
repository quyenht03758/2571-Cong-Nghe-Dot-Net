namespace K8sManager.Api.Domain.Entities
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Category { get; set; } // Workload, Network, Storage, Config
        public string? Description { get; set; }
        public string? Tags { get; set; } // Comma-separated
        public bool IsPublic { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public AppUser? Creator { get; set; }
        public List<TemplateVersion>? Versions { get; set; }
    }
}

