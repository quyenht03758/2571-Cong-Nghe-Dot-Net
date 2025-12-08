// Application/AuditLogs/Queries/GetAuditLogsQuery.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.AuditLogs.Queries;

public record GetAuditLogsQuery(
    int? UserId = null,
    string? Action = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int Skip = 0,
    int Take = 100
);

public class GetAuditLogsQueryHandler
{
    private readonly IAuditLogRepository _auditRepo;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task<List<AuditLog>> HandleAsync(GetAuditLogsQuery query)
    {
        var logs = await _auditRepo.GetLogsAsync(
            query.UserId,
            null, // clusterId
            query.Action,
            query.StartDate,
            query.EndDate
        ).ConfigureAwait(false);
        return logs.ToList();
    }
}
