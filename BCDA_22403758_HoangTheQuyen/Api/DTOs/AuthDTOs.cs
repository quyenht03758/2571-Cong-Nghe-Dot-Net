// Api/DTOs/AuthDTOs.cs - Authentication & Authorization DTOs
namespace K8sManager.Api.DTOs;

// ============ REQUEST DTOs ============

public record LoginRequest(string Username, string Password);

public record RegisterUserRequest(
    string Username,
    string Password,
    string Email,
    string? FullName = null
);

public record ChangePasswordRequest(
    string OldPassword,
    string NewPassword
);

// ============ RESPONSE DTOs ============

public record UserInfo(
    int Id,
    string Username,
    string Email,
    string? FullName,
    string Role,
    DateTime CreatedAt,
    bool IsActive
);

public class UserInfoDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = "Viewer";
}

public class UpdateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public UserInfoDTO? User { get; set; }
}

