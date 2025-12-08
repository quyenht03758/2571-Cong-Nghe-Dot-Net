// Presentation/Controllers/UsersController.cs
using System.Security.Claims;
using K8sManager.Api.Application.Users;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly CreateUserCommandHandler _createHandler;
    private readonly UpdateUserCommandHandler _updateHandler;
    private readonly ChangePasswordCommandHandler _changePasswordHandler;
    private readonly ResetPasswordCommandHandler _resetPasswordHandler;
    private readonly LockUserCommandHandler _lockHandler;
    private readonly UnlockUserCommandHandler _unlockHandler;
    private readonly DeleteUserCommandHandler _deleteHandler;
    private readonly GetUsersQueryHandler _getUsersHandler;
    private readonly GetUserByIdQueryHandler _getByIdHandler;

    public UsersController(
        CreateUserCommandHandler createHandler,
        UpdateUserCommandHandler updateHandler,
        ChangePasswordCommandHandler changePasswordHandler,
        ResetPasswordCommandHandler resetPasswordHandler,
        LockUserCommandHandler lockHandler,
        UnlockUserCommandHandler unlockHandler,
        DeleteUserCommandHandler deleteHandler,
        GetUsersQueryHandler getUsersHandler,
        GetUserByIdQueryHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _changePasswordHandler = changePasswordHandler;
        _resetPasswordHandler = resetPasswordHandler;
        _lockHandler = lockHandler;
        _unlockHandler = unlockHandler;
        _deleteHandler = deleteHandler;
        _getUsersHandler = getUsersHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserResponse>>> GetAll([FromQuery] bool? isActive = null)
    {
        var query = new GetUsersQuery(isActive);
        var users = await _getUsersHandler.HandleAsync(query).ConfigureAwait(false);
        
        var response = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email?.Value, // Extract string from Email ValueObject
            DisplayName = u.DisplayName,
            FullName = u.FullName,
            Role = u.Role.ToString(), // Convert enum to string
            IsLocked = u.IsLocked,
            IsActive = u.IsActive,
            FailedLoginAttempts = u.FailedLoginAttempts,
            LastLoginAt = u.LastLoginAt,
            LastPasswordChangedAt = u.LastPasswordChangedAt,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var query = new GetUserByIdQuery(id);
        var user = await _getByIdHandler.HandleAsync(query).ConfigureAwait(false);

        if (user == null)
            return NotFound(new { Message = "User not found" });

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email?.Value, // Extract string from Email ValueObject
            DisplayName = user.DisplayName,
            FullName = user.FullName,
            Role = user.Role.ToString(), // Convert enum to string
            IsLocked = user.IsLocked,
            IsActive = user.IsActive,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LastLoginAt = user.LastLoginAt,
            LastPasswordChangedAt = user.LastPasswordChangedAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.Username,
            request.Password,
            request.Email,
            request.FullName,
            request.Role
        );

        var id = await _createHandler.HandleAsync(command).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(id, request.Email, request.FullName, request.Role);
        var result = await _updateHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "User not found" });

        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        var command = new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword);
        var result = await _changePasswordHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return BadRequest(new { Message = "Failed to change password. Check old password." });

        return Ok(new { Message = "Password changed successfully" });
    }

    [HttpPost("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
    {
        var adminUserId = GetUserId();
        var command = new ResetPasswordCommand(id, request.NewPassword, adminUserId);
        var result = await _resetPasswordHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "Password reset successfully" });
    }

    [HttpPost("{id}/lock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Lock(int id)
    {
        var adminUserId = GetUserId();
        var command = new LockUserCommand(id, adminUserId);
        var result = await _lockHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "User locked successfully" });
    }

    [HttpPost("{id}/unlock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Unlock(int id)
    {
        var adminUserId = GetUserId();
        var command = new UnlockUserCommand(id, adminUserId);
        var result = await _unlockHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "User not found" });

        return Ok(new { Message = "User unlocked successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var adminUserId = GetUserId();
        var command = new DeleteUserCommand(id, adminUserId);
        var result = await _deleteHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "User not found" });

        return NoContent();
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
