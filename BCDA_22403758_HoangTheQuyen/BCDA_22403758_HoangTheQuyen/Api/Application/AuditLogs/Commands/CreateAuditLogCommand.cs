// Application/AuditLogs/Commands/CreateAuditLogCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.AuditLogs.Commands;

public record CreateAuditLogCommand(
    int? UserId,
    string Action,
    string ResourceKind,
    string ResourceName,
    bool Success,
    string? ErrorMessage = null,
    string? Details = null
);

public class CreateAuditLogCommandHandler
{
    private readonly IAuditLogRepository _auditRepo;

    public CreateAuditLogCommandHandler(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task<int> HandleAsync(CreateAuditLogCommand command)
    {
        var auditLog = new AuditLog
        {
            UserId = command.UserId ?? 0,
            Action = command.Action,
            ResourceKind = command.ResourceKind,
            ResourceName = command.ResourceName,
            Success = command.Success,
            ErrorMessage = command.ErrorMessage,
            // Details removed - AuditLog entity doesn't have this property
            CreatedAt = DateTime.UtcNow
        };

        var id = await _auditRepo.CreateAsync(auditLog).ConfigureAwait(false);
        return (int)id;
    }
}
