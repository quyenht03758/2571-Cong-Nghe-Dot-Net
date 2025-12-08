namespace K8sManager.Api.DTOs;

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

// K8s Resource DTOs
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
