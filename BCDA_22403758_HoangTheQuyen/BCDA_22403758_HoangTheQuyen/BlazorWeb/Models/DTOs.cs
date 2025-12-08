namespace BlazorWeb.Models;

// YAML Response DTO
public record YamlResponse(string Yaml);

// Auth DTOs
public record LoginRequest(string Username, string Password);

public record LoginResponse(
    int UserId,
    string Username,
    string Email,
    string Role,
    string Token,
    DateTime ExpiresAt
);

// User DTOs
public record UserDto(
    int Id,
    string Username,
    string Email,
    string? DisplayName,
    string Role,
    bool IsLocked,
    DateTime CreatedAt
);

// Template DTOs
public record TemplateDto(
    int Id,
    string Name,
    string? Description,
    string? Category,
    string? Tags,
    bool IsPublic,
    string? YamlContent,
    int Version,
    int CreatedBy,
    DateTime CreatedAt
);

public record CreateTemplateRequest(
    string Name,
    string YamlContent,
    string? Description,
    string? Category,
    string? Tags,
    bool IsPublic
);

public record UpdateTemplateRequest(
    string Name,
    string YamlContent,
    string? Description,
    string? Category
);

// Favorite DTOs
public record FavoriteDto(
    int Id,
    int UserId,
    int ClusterId,
    string? ResourceKind,
    string? ResourceName,
    string? Namespace,
    string DisplayName,
    string? Notes,
    int SortOrder,
    DateTime CreatedAt
);

public record CreateFavoriteRequest(
    string ResourceType,
    string ResourceName,
    string Namespace,
    string? Notes,
    string? ClusterName = null
);

// Audit Log DTOs
public record AuditLogDto(
    long Id,
    int UserId,
    string Username,
    string Action,
    string? ResourceKind,
    string? ResourceName,
    string? Namespace,
    bool Success,
    string? ErrorMessage,
    string? IpAddress,
    DateTime CreatedAt
);

// Session DTOs
public record SessionDto(
    long Id,
    int UserId,
    string SessionToken,
    string? IpAddress,
    string? UserAgent,
    DateTime ExpiresAt,
    DateTime CreatedAt
);

// Kubernetes Resource DTOs (simplified)
public record PodDto(
    string Name,
    string Namespace,
    string Status,
    int Restarts,
    DateTime? CreatedAt,
    Dictionary<string, string>? Labels
);

public record DeploymentDto(
    string Name,
    string Namespace,
    int Replicas,
    int ReadyReplicas,
    DateTime? CreatedAt
);

public record ServiceDto(
    string Name,
    string Namespace,
    string Type,
    string ClusterIP,
    string[]? Ports
);

public record NamespaceDto(
    string Name,
    string Status,
    DateTime? CreatedAt
);

// Cluster DTOs
public record ClusterDto(
    int Id,
    string Name,
    string KubeconfigPath,
    string ContextName,
    bool IsDefault,
    string? Environment,
    string? Description,
    DateTime CreatedAt
);

public record CreateClusterRequest(
    string Name,
    string KubeconfigPath,
    string? ContextName,
    bool IsDefault,
    string? Environment,
    string? Description
);

// Enhanced K8s Resource DTOs
public record K8sNamespaceDto(
    string Name,
    string Status,
    DateTime CreatedAt,
    Dictionary<string, string>? Labels
);

public record K8sPodDto(
    string Name,
    string Namespace,
    string Status,
    string Phase,
    int RestartCount,
    string? PodIP,
    string? NodeName,
    DateTime CreatedAt,
    Dictionary<string, string>? Labels,
    List<K8sContainerDto>? Containers
);

public record K8sContainerDto(
    string Name,
    string Image,
    bool Ready,
    int RestartCount
);

public record K8sDeploymentDto(
    string Name,
    string Namespace,
    int Replicas,
    int ReadyReplicas,
    int AvailableReplicas,
    DateTime CreatedAt,
    Dictionary<string, string>? Labels
);

public record K8sServiceDto(
    string Name,
    string Namespace,
    string Type,
    string? ClusterIP,
    List<string>? ExternalIPs,
    List<string>? Ports,
    DateTime CreatedAt,
    Dictionary<string, string>? Labels
);

public record K8sConfigMapDto(
    string Name,
    string Namespace,
    int DataCount,
    DateTime CreatedAt
);

public record K8sSecretDto(
    string Name,
    string Namespace,
    string Type,
    int DataCount,
    DateTime CreatedAt
);

public record ApplyYamlRequest(
    string YamlContent
);

public record ApplyYamlResponse(
    bool Success,
    string Message,
    List<string>? CreatedResources
);

public record CreateNamespaceRequest(
    string Name
);

// User Management DTOs
public record UserResponse(
    int Id,
    string Username,
    string? Email,
    string? DisplayName,
    string? FullName,
    string Role,
    bool IsLocked,
    bool IsActive,
    int FailedLoginAttempts,
    DateTime? LastLoginAt,
    DateTime? LastPasswordChangedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class CreateUserRequestDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public string Role { get; set; } = "Viewer";
}

public class UpdateUserRequestDto
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public string Role { get; set; } = "";
}

public record ResetPasswordRequestDto(
    string NewPassword
);
