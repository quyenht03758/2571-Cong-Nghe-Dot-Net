using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long id);

    Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        int? userId = null,
        int? clusterId = null,
        string? action = null,
        bool? success = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    Task<long> CreateAsync(AuditLog auditLog);

    Task<IEnumerable<AuditLog>> GetRecentByUserAsync(int userId, int count = 10);

    Task<IEnumerable<AuditLog>> GetFailedActionsAsync(int? userId = null, DateTime? since = null, int limit = 100);

    Task<Dictionary<string, int>> GetActionStatisticsAsync(DateTime fromDate, DateTime toDate);

    Task<IEnumerable<AuditLog>> GetLogsAsync(
        int? userId = null,
        int? clusterId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}

