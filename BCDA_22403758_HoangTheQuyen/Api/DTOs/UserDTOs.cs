// DTOs/UserDTOs.cs - User Management Response DTOs
namespace K8sManager.Api.DTOs;

/// <summary>
/// User response DTO for API - Maps User entity to JSON-friendly format
/// Converts Email ValueObject to string and Role enum to string
/// </summary>
public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? FullName { get; set; }
    public string Role { get; set; } = "";
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastPasswordChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Note: CreateUserRequest, UpdateUserRequest, ResetPasswordRequest, ChangePasswordRequest
// are already defined in AuthDTOs.cs
