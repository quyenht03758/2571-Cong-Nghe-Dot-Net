// Application/AuditLogs/Queries/GetAuditLogByIdQuery.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.AuditLogs.Queries;

public record GetAuditLogByIdQuery(int Id);

public class GetAuditLogByIdQueryHandler
{
    private readonly IAuditLogRepository _auditRepo;

    public GetAuditLogByIdQueryHandler(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task<AuditLog?> HandleAsync(GetAuditLogByIdQuery query)
    {
        return await _auditRepo.GetByIdAsync(query.Id).ConfigureAwait(false);
    }
}
