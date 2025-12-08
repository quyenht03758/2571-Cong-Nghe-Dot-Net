// Application/Auth/Commands/LoginCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Domain.Services;

namespace K8sManager.Api.Application.Auth.Commands;

public record LoginCommand(string Username, string Password);

public record LoginResult(
    bool Success,
    string? Token,
    UserInfo? User,
    string? Message
);

public record UserInfo(
    int Id,
    string Username,
    string Email,
    string? FullName,
    string Role,
    DateTime CreatedAt,
    bool IsActive
);

public class LoginCommandHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IUserSessionRepository _sessionRepo;
    private readonly IAuditLogRepository _auditRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepo,
        IUserSessionRepository sessionRepo,
        IAuditLogRepository auditRepo,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepo = userRepo;
        _sessionRepo = sessionRepo;
        _auditRepo = auditRepo;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResult> HandleAsync(LoginCommand command)
    {
        try
        {
            // 1. Find user
            var user = await _userRepo.FindByUsernameAsync(command.Username).ConfigureAwait(false);
            if (user == null)
            {
                await LogFailedLoginAsync(null, command.Username, "User not found").ConfigureAwait(false);
                return new LoginResult(false, null, null, "Invalid username or password");
            }

            // 2. Check if user is locked
            if (!user.IsActive)
            {
                await LogFailedLoginAsync(user.Id, command.Username, "Account is locked").ConfigureAwait(false);
                return new LoginResult(false, null, null, "Account is locked. Contact administrator.");
            }

            // 3. Verify password
            if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            {
                await LogFailedLoginAsync(user.Id, command.Username, "Invalid password").ConfigureAwait(false);
                return new LoginResult(false, null, null, "Invalid username or password");
            }

            // 4. Generate JWT token
            var token = _tokenGenerator.Generate(user.Id, user.Username, user.Role.ToString());

            // 5. Create session
            var session = new UserSession
            {
                UserId = user.Id,
                SessionToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                CreatedAt = DateTime.UtcNow
            };
            await _sessionRepo.CreateAsync(session).ConfigureAwait(false);

            // 6. Log successful login
            await _auditRepo.CreateAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                ResourceKind = "Auth",
                ResourceName = user.Username,
                Success = true,
                CreatedAt = DateTime.UtcNow
            }).ConfigureAwait(false);

            // 7. Return success
            return new LoginResult(
                Success: true,
                Token: token,
                User: new UserInfo(
                    user.Id,
                    user.Username,
                    user.Email?.Value ?? "",
                    user.FullName,
                    user.Role.ToString(),
                    user.CreatedAt,
                    user.IsActive
                ),
                Message: "Login successful"
            );
        }
        catch (Exception ex)
        {
            await LogFailedLoginAsync(null, command.Username, $"Exception: {ex.Message}").ConfigureAwait(false);
            return new LoginResult(false, null, null, "An error occurred during login");
        }
    }

    private async Task LogFailedLoginAsync(int? userId, string username, string reason)
    {
        await _auditRepo.CreateAsync(new AuditLog
        {
            UserId = userId ?? 0,
            Action = "Login",
            ResourceKind = "Auth",
            ResourceName = username,
            Success = false,
            ErrorMessage = reason,
            CreatedAt = DateTime.UtcNow
        }).ConfigureAwait(false);
    }
}
