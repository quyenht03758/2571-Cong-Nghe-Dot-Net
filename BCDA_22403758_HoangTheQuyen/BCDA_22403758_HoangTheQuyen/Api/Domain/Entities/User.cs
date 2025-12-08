// Api/Domain/Entities/User.cs
using K8sManager.Api.Domain.Exceptions;
using K8sManager.Api.Domain.Services;
using K8sManager.Api.Domain.ValueObjects;

namespace K8sManager.Api.Domain.Entities;

/// <summary>
/// User domain entity with rich behavior
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public Email? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? FullName { get; set; }
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; } = true;
    public int FailedLoginAttempts { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastPasswordChangedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Public constructor
    public User() { }

    // Factory method for creating new user
    public static User Create(
        string username,
        Password password,
        IPasswordHasher passwordHasher,
        UserRole role = UserRole.Viewer,
        Email? email = null,
        string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidUsernameException("Username cannot be empty");

        if (username.Length < 3)
            throw new InvalidUsernameException("Username must be at least 3 characters");

        if (username.Length > 100)
            throw new InvalidUsernameException("Username cannot exceed 100 characters");

        var now = DateTime.UtcNow;

        return new User
        {
            Username = username.Trim(),
            Email = email,
            DisplayName = displayName?.Trim(),
            PasswordHash = passwordHasher.HashPassword(password.Value),
            Role = role,
            IsLocked = false,
            FailedLoginAttempts = 0,
            LastPasswordChangedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    // Factory method for loading from database
    public static User Load(
        int id,
        string username,
        string? email,
        string? displayName,
        string passwordHash,
        string role,
        bool isLocked,
        int failedLoginAttempts,
        DateTime? lastLoginAt,
        DateTime? lastPasswordChangedAt,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new User
        {
            Id = id,
            Username = username,
            Email = string.IsNullOrWhiteSpace(email) ? null : Email.Create(email),
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = Enum.Parse<UserRole>(role),
            IsLocked = isLocked,
            FailedLoginAttempts = failedLoginAttempts,
            LastLoginAt = lastLoginAt,
            LastPasswordChangedAt = lastPasswordChangedAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    /// <summary>
    /// Change user password
    /// </summary>
    public void ChangePassword(Password newPassword, IPasswordHasher passwordHasher)
    {
        PasswordHash = passwordHasher.HashPassword(newPassword.Value);
        LastPasswordChangedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verify password
    /// </summary>
    public bool VerifyPassword(string plainPassword, IPasswordHasher passwordHasher)
    {
        if (IsLocked)
            throw new UserLockedException(Id);

        return passwordHasher.VerifyPassword(plainPassword, PasswordHash);
    }

    /// <summary>
    /// Record successful login
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Record failed login attempt
    /// </summary>
    public void RecordFailedLogin(int maxAttempts = 5)
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        if (FailedLoginAttempts >= maxAttempts)
        {
            Lock();
        }
    }

    /// <summary>
    /// Lock user account
    /// </summary>
    public void Lock()
    {
        IsLocked = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unlock user account
    /// </summary>
    public void Unlock()
    {
        IsLocked = false;
        FailedLoginAttempts = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    public void UpdateProfile(Email? email, string? displayName)
    {
        Email = email;
        DisplayName = displayName?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Change user role (Admin only operation)
    /// </summary>
    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if user has permission
    /// </summary>
    public bool HasPermission(Permission permission)
    {
        return Role switch
        {
            UserRole.Admin => true,
            UserRole.Operator => permission is not Permission.ManageUsers and not Permission.ManageSettings,
            UserRole.Viewer => permission is Permission.ViewResources or Permission.ViewAuditLogs,
            _ => false
        };
    }
}

/// <summary>
/// User roles enumeration
/// </summary>
public enum UserRole
{
    Viewer,
    Operator,
    Admin
}

/// <summary>
/// Permission enumeration
/// </summary>
public enum Permission
{
    ViewResources,
    ViewAuditLogs,
    CreateResources,
    UpdateResources,
    DeleteResources,
    ManageUsers,
    ManageSettings
}

