// Api/DTOs/AuditDTOs.cs
namespace K8sManager.Api.DTOs;

public class CreateAuditLogRequest
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceKind { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Details { get; set; }
}

public record AuditLogRequest(
    int UserId,
    string Action,
    string ResourceKind,
    string ResourceName,
    string Namespace,
    bool Success,
    string? ErrorMessage = null
);

public record AuditLogResponse(
    long Id,
    int UserId,
    string Username,
    string Action,
    string ResourceKind,
    string ResourceName,
    string Namespace,
    bool Success,
    string? ErrorMessage,
    string? IpAddress,
    DateTime Timestamp
);

public record AuditLogFilter(
    int? UserId = null,
    string? Action = null,
    string? ResourceKind = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    bool? Success = null,
    int PageNumber = 1,
    int PageSize = 50
);

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

