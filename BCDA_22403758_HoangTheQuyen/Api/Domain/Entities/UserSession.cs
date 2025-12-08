namespace K8sManager.Api.Domain.Entities;

public class UserSession
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = "Viewer"; // Default role

    // Navigation
    public AppUser? User { get; set; }
}

