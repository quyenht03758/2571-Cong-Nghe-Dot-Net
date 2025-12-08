// Presentation/Controllers/AuditLogsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Application.AuditLogs.Commands;
using K8sManager.Api.Application.AuditLogs.Queries;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly CreateAuditLogCommandHandler _createHandler;
    private readonly GetAuditLogsQueryHandler _getLogsHandler;
    private readonly GetAuditLogByIdQueryHandler _getByIdHandler;

    public AuditLogsController(
        CreateAuditLogCommandHandler createHandler,
        GetAuditLogsQueryHandler getLogsHandler,
        GetAuditLogByIdQueryHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _getLogsHandler = getLogsHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<List<AuditLogResponse>>> GetLogs(
        [FromQuery] int? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100)
    {
        var query = new GetAuditLogsQuery(userId, action, startDate, endDate, skip, take);
        var logs = await _getLogsHandler.HandleAsync(query).ConfigureAwait(false);
        
        var response = logs.Select(log => new AuditLogResponse(
            log.Id,
            log.UserId,
            log.User?.Username ?? "Unknown",
            log.Action,
            log.ResourceKind ?? "",
            log.ResourceName ?? "",
            log.Namespace ?? "",
            log.Success,
            log.ErrorMessage,
            log.IpAddress,
            log.CreatedAt
        )).ToList();
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<AuditLog>> GetById(int id)
    {
        var query = new GetAuditLogByIdQuery(id);
        var log = await _getByIdHandler.HandleAsync(query).ConfigureAwait(false);

        if (log == null)
            return NotFound(new { Message = "Audit log not found" });

        return Ok(log);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateAuditLogRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdClaim != null && int.TryParse(userIdClaim, out int uid) ? uid : null;

        var command = new CreateAuditLogCommand(
            userId ?? request.UserId,
            request.Action,
            request.ResourceKind,
            request.ResourceName,
            request.Success,
            request.ErrorMessage,
            request.Details
        );

        var id = await _createHandler.HandleAsync(command).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }
}
