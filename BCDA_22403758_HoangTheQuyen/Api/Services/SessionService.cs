using System.Security.Cryptography;
using System.Text;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Services;

public class SessionService
{
    private readonly IUserSessionRepository _sessionRepo;
    private readonly IAppUserRepository _userRepo;
    private readonly IAuditRepository _auditRepo;
    private AppUser? _currentUser;
    private string? _currentSessionToken;

    public SessionService(
        IUserSessionRepository sessionRepo,
        IAppUserRepository userRepo,
        IAuditRepository auditRepo)
    {
        _sessionRepo = sessionRepo;
        _userRepo = userRepo;
        _auditRepo = auditRepo;
    }

    public AppUser? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    /// <summary>
    /// Authenticates user with username and password
    /// </summary>
    public async Task<(bool Success, string Message, AppUser? User, string? SessionToken)> LoginAsync(string username, string password, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var user = await _userRepo.GetByUsernameAsync(username).ConfigureAwait(false);
            if (user == null)
            {
                await _auditRepo.LogAsync(new AuditLog
                {
                    UserId = 0,
                    Action = "Login",
                    Success = false,
                    ErrorMessage = $"User '{username}' not found",
                    IpAddress = ipAddress
                }).ConfigureAwait(false);
                return (false, "Invalid username or password", null, null);
            }

            // Verify password (in production, use BCrypt or proper password hashing)
            if (!VerifyPassword(password, user.PasswordHash))
            {
                // Just log failed attempt, don't increment or lock
                await _auditRepo.LogAsync(new AuditLog
                {
                    UserId = user.Id,
                    Action = "Login",
                    Success = false,
                    ErrorMessage = "Invalid password",
                    IpAddress = ipAddress
                }).ConfigureAwait(false);

                return (false, "Invalid username or password", null, null);
            }

            // Successful login - reset failed attempts
            user.FailedLoginAttempts = 0;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user).ConfigureAwait(false);

            // Create session
            var sessionToken = GenerateSessionToken();
            var session = new UserSession
            {
                UserId = user.Id,
                SessionToken = sessionToken,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                ExpiresAt = DateTime.UtcNow.AddHours(8) // 8 hours session
            };

            await _sessionRepo.CreateAsync(session).ConfigureAwait(false);

            _currentUser = user;
            _currentSessionToken = sessionToken;

            await _auditRepo.LogAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                Success = true,
                ErrorMessage = "Login successful",
                IpAddress = ipAddress
            }).ConfigureAwait(false);

            return (true, "Login successful", user, sessionToken);
        }
        catch (Exception ex)
        {
            await _auditRepo.LogAsync(new AuditLog
            {
                UserId = 0,
                Action = "Login",
                Success = false,
                ErrorMessage = $"Login error: {ex.Message}",
                IpAddress = ipAddress
            }).ConfigureAwait(false);
            return (false, $"Login failed: {ex.Message}", null, null);
        }
    }

    /// <summary>
    /// Validates session token and loads current user
    /// </summary>
    public async Task<bool> ValidateSessionAsync(string sessionToken)
    {
        try
        {
            var session = await _sessionRepo.GetByTokenAsync(sessionToken).ConfigureAwait(false);
            if (session == null)
                return false;

            var user = await _userRepo.GetByIdAsync(session.UserId).ConfigureAwait(false);
            if (user == null || user.IsLocked)
                return false;

            _currentUser = user;
            _currentSessionToken = sessionToken;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Logs out current user
    /// </summary>
    public async Task LogoutAsync()
    {
        if (_currentUser != null && _currentSessionToken != null)
        {
            await _sessionRepo.DeleteByUserIdAsync(_currentUser.Id).ConfigureAwait(false);

            await _auditRepo.LogAsync(new AuditLog
            {
                UserId = _currentUser.Id,
                Action = "Logout",
                Success = true
            }).ConfigureAwait(false);
        }

        _currentUser = null;
        _currentSessionToken = null;
    }

    /// <summary>
    /// Checks if current user has required role
    /// </summary>
    public bool HasRole(params string[] roles)
    {
        if (_currentUser == null)
            return false;

        return roles.Contains(_currentUser.Role);
    }

    /// <summary>
    /// Checks if current user can perform write operations
    /// </summary>
    public bool CanWrite()
    {
        return HasRole("Admin", "Operator");
    }

    /// <summary>
    /// Checks if current user can delete resources
    /// </summary>
    public bool CanDelete()
    {
        return HasRole("Admin");
    }

    private string GenerateSessionToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        // Check if it's a BCrypt hash
        if (hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$"))
        {
            try
            {
                // Use BCrypt to verify
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        // Fallback: plain text comparison (for demo/testing only)
        // Remove this in production!
        return hash == password;
    }

    private string HashPassword(string password)
    {
        // Simple SHA256 for demo - in production use BCrypt
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Cleans up expired sessions
    /// </summary>
    public async Task CleanupExpiredSessionsAsync()
    {
        await _sessionRepo.DeleteExpiredAsync().ConfigureAwait(false);
    }
}
