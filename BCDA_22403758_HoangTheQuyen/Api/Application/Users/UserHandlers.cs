// Application/Users/UserHandlers.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.Domain.Services;

namespace K8sManager.Api.Application.Users;

// Commands
public record CreateUserCommand(string Username, string Password, string Email, string? FullName, string Role);
public record UpdateUserCommand(int Id, string Email, string? FullName, string Role);
public record ChangePasswordCommand(int UserId, string OldPassword, string NewPassword);
public record ResetPasswordCommand(int UserId, string NewPassword, int AdminUserId);
public record LockUserCommand(int UserId, int AdminUserId);
public record UnlockUserCommand(int UserId, int AdminUserId);
public record DeleteUserCommand(int UserId, int AdminUserId);

// Queries
public record GetUsersQuery(bool? IsActive = null);
public record GetUserByIdQuery(int Id);

// Handlers
public class CreateUserCommandHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepo, IPasswordHasher passwordHasher)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> HandleAsync(CreateUserCommand command)
    {
        var user = new User
        {
            Username = command.Username,
            PasswordHash = _passwordHasher.Hash(command.Password),
            Email = string.IsNullOrWhiteSpace(command.Email) ? null : Domain.ValueObjects.Email.Create(command.Email),
            FullName = command.FullName,
            Role = Enum.Parse<Domain.Entities.UserRole>(command.Role),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await _userRepo.CreateAsync(user).ConfigureAwait(false);
    }
}

public class UpdateUserCommandHandler
{
    private readonly IUserRepository _userRepo;

    public UpdateUserCommandHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<bool> HandleAsync(UpdateUserCommand command)
    {
        var user = await _userRepo.FindByIdAsync(command.Id).ConfigureAwait(false);
        if (user == null) return false;

        user.Email = string.IsNullOrWhiteSpace(command.Email) ? null : Domain.ValueObjects.Email.Create(command.Email);
        user.FullName = command.FullName;
        user.Role = Enum.Parse<Domain.Entities.UserRole>(command.Role);
        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepo.UpdateAsync(user).ConfigureAwait(false);
    }
}

public class ChangePasswordCommandHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IUserRepository userRepo, IPasswordHasher passwordHasher)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> HandleAsync(ChangePasswordCommand command)
    {
        var user = await _userRepo.FindByIdAsync(command.UserId).ConfigureAwait(false);
        if (user == null) return false;

        if (!_passwordHasher.Verify(command.OldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepo.UpdateAsync(user).ConfigureAwait(false);
    }
}

public class ResetPasswordCommandHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IUserRepository userRepo, IPasswordHasher passwordHasher)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
    }

    public async Task<bool> HandleAsync(ResetPasswordCommand command)
    {
        var user = await _userRepo.FindByIdAsync(command.UserId).ConfigureAwait(false);
        if (user == null) return false;

        user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepo.UpdateAsync(user).ConfigureAwait(false);
    }
}

public class LockUserCommandHandler
{
    private readonly IUserRepository _userRepo;

    public LockUserCommandHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<bool> HandleAsync(LockUserCommand command)
    {
        var user = await _userRepo.FindByIdAsync(command.UserId).ConfigureAwait(false);
        if (user == null) return false;

        user.IsLocked = true;
        return await _userRepo.UpdateAsync(user).ConfigureAwait(false);
    }
}

public class UnlockUserCommandHandler
{
    private readonly IUserRepository _userRepo;

    public UnlockUserCommandHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<bool> HandleAsync(UnlockUserCommand command)
    {
        var user = await _userRepo.FindByIdAsync(command.UserId).ConfigureAwait(false);
        if (user == null) return false;

        user.IsLocked = false;
        return await _userRepo.UpdateAsync(user).ConfigureAwait(false);
    }
}

public class DeleteUserCommandHandler
{
    private readonly IUserRepository _userRepo;

    public DeleteUserCommandHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<bool> HandleAsync(DeleteUserCommand command)
    {
        return await _userRepo.DeleteAsync(command.UserId).ConfigureAwait(false);
    }
}

public class GetUsersQueryHandler
{
    private readonly IUserRepository _userRepo;

    public GetUsersQueryHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<List<User>> HandleAsync(GetUsersQuery query)
    {
        var users = await _userRepo.GetAllAsync().ConfigureAwait(false);
        return users.ToList();
    }
}

public class GetUserByIdQueryHandler
{
    private readonly IUserRepository _userRepo;

    public GetUserByIdQueryHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<User?> HandleAsync(GetUserByIdQuery query)
    {
        return await _userRepo.FindByIdAsync(query.Id).ConfigureAwait(false);
    }
}
