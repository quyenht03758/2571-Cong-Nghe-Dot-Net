// Presentation/Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Application.Auth.Commands;
using K8sManager.Api.Application.Auth.Queries;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginCommandHandler _loginHandler;
    private readonly GetCurrentUserQueryHandler _getCurrentUserHandler;

    public AuthController(
        LoginCommandHandler loginHandler,
        GetCurrentUserQueryHandler getCurrentUserHandler)
    {
        _loginHandler = loginHandler;
        _getCurrentUserHandler = getCurrentUserHandler;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var result = await _loginHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result.Success)
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = result.Message ?? "Login failed"
            });
        }

        return Ok(new LoginResponse
        {
            Success = true,
            Token = result.Token!,
            Message = result.Message!,
            User = result.User == null ? null : new UserInfoDTO
            {
                Id = result.User.Id,
                Username = result.User.Username,
                Email = result.User.Email,
                FullName = result.User.FullName,
                Role = result.User.Role,
                CreatedAt = result.User.CreatedAt,
                IsActive = result.User.IsActive
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfoDTO>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { Message = "Invalid token" });
        }

        var query = new GetCurrentUserQuery(userId);
        var user = await _getCurrentUserHandler.HandleAsync(query).ConfigureAwait(false);

        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        return Ok(new UserInfoDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            IsActive = user.IsActive
        });
    }
}
