namespace K8sManager.Api.Domain.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Viewer"; // Admin, Operator, Viewer
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChangedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

