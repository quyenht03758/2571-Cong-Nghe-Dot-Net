// Presentation/Controllers/SessionsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Application.Sessions;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly DeleteSessionCommandHandler _deleteHandler;
    private readonly GetActiveSessionsQueryHandler _getActiveHandler;
    private readonly GetMySessionsQueryHandler _getMySessionsHandler;
    private readonly CleanupExpiredSessionsHandler _cleanupHandler;

    public SessionsController(
        DeleteSessionCommandHandler deleteHandler,
        GetActiveSessionsQueryHandler getActiveHandler,
        GetMySessionsQueryHandler getMySessionsHandler,
        CleanupExpiredSessionsHandler cleanupHandler)
    {
        _deleteHandler = deleteHandler;
        _getActiveHandler = getActiveHandler;
        _getMySessionsHandler = getMySessionsHandler;
        _cleanupHandler = cleanupHandler;
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserSession>>> GetActiveSessions([FromQuery] int? userId = null)
    {
        var query = new GetActiveSessionsQuery(userId);
        var sessions = await _getActiveHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(sessions);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<UserSession>>> GetMySessions()
    {
        var userId = GetUserId();
        var query = new GetMySessionsQuery(userId);
        var sessions = await _getMySessionsHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(sessions);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var command = new DeleteSessionCommand(token);
        await _deleteHandler.HandleAsync(command).ConfigureAwait(false);
        return Ok(new { Message = "Logged out successfully" });
    }

    [HttpPost("cleanup")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> CleanupExpired()
    {
        var command = new CleanupExpiredSessionsCommand();
        var deletedCount = await _cleanupHandler.HandleAsync(command).ConfigureAwait(false);
        return Ok(new { DeletedCount = deletedCount });
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
