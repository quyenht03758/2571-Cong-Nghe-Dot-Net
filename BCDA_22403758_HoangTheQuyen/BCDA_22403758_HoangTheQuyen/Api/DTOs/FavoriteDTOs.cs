// Api/DTOs/FavoriteDTOs.cs
namespace K8sManager.Api.DTOs;

public class CreateFavoriteRequest
{
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string? ClusterName { get; set; }
    public string? Namespace { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFavoriteRequest
{
    public string? Notes { get; set; }
}

public record FavoriteRequest(
    string ResourceKind,
    string ResourceName,
    string Namespace,
    string? ClusterName = null,
    string? Notes = null
);

public record FavoriteResponse(
    int Id,
    int UserId,
    string ResourceKind,
    string ResourceName,
    string Namespace,
    string? ClusterName,
    string? Notes,
    DateTime CreatedAt
);

public record FavoriteFilter(
    int? UserId = null,
    string? ResourceKind = null,
    string? Namespace = null,
    string? ClusterName = null,
    int PageNumber = 1,
    int PageSize = 50
);

