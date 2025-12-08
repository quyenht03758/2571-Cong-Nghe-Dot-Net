// Application/Favorites/FavoriteHandlers.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace K8sManager.Api.Application.Favorites;

// Commands
public record CreateFavoriteCommand(int UserId, string ResourceType, string ResourceName, string? ClusterName, string? Namespace, string? Notes);
public record UpdateFavoriteCommand(int Id, string? Notes);
public record DeleteFavoriteCommand(int Id, int UserId);

// Queries
public record GetFavoritesQuery(int UserId);

// Handlers
public class CreateFavoriteCommandHandler
{
    private readonly IFavoriteRepository _favoriteRepo;
    private readonly ILogger<CreateFavoriteCommandHandler> _logger;

    public CreateFavoriteCommandHandler(IFavoriteRepository favoriteRepo, ILogger<CreateFavoriteCommandHandler> logger)
    {
        _favoriteRepo = favoriteRepo;
        _logger = logger;
    }

    public async Task<int> HandleAsync(CreateFavoriteCommand command)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(command.ResourceType))
            throw new ArgumentException("ResourceType cannot be empty", nameof(command.ResourceType));
        
        if (string.IsNullOrWhiteSpace(command.ResourceName))
            throw new ArgumentException("ResourceName cannot be empty", nameof(command.ResourceName));

        try
        {
            _logger.LogInformation("Creating favorite for user {UserId}: {ResourceType}/{ResourceName} in {Namespace}",
                command.UserId, command.ResourceType, command.ResourceName, command.Namespace);

            var favorite = new Favorite
            {
                UserId = command.UserId,
                ClusterId = 1, // Default cluster - TODO: get from context
                ResourceKind = command.ResourceType, // Map ResourceType to ResourceKind for database
                ResourceName = command.ResourceName,
                DisplayName = $"{command.ResourceType}/{command.ResourceName}",
                Namespace = command.Namespace,
                Notes = command.Notes,
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow
            };
            
            var id = await _favoriteRepo.CreateAsync(favorite).ConfigureAwait(false);
            
            _logger.LogInformation("Successfully created favorite {FavoriteId} for user {UserId}", id, command.UserId);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating favorite for user {UserId}: {ResourceType}/{ResourceName}",
                command.UserId, command.ResourceType, command.ResourceName);
            throw;
        }
    }
}

public class UpdateFavoriteCommandHandler
{
    private readonly IFavoriteRepository _favoriteRepo;
    private readonly ILogger<UpdateFavoriteCommandHandler> _logger;

    public UpdateFavoriteCommandHandler(IFavoriteRepository favoriteRepo, ILogger<UpdateFavoriteCommandHandler> logger)
    {
        _favoriteRepo = favoriteRepo;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(UpdateFavoriteCommand command)
    {
        try
        {
            _logger.LogInformation("Updating favorite {FavoriteId}", command.Id);

            var favorite = await _favoriteRepo.GetByIdAsync(command.Id).ConfigureAwait(false);
            if (favorite == null)
            {
                _logger.LogWarning("Favorite {FavoriteId} not found for update", command.Id);
                return false;
            }

            favorite.Notes = command.Notes;
            var result = await _favoriteRepo.UpdateAsync(favorite).ConfigureAwait(false);

            if (result)
                _logger.LogInformation("Successfully updated favorite {FavoriteId}", command.Id);
            else
                _logger.LogWarning("Failed to update favorite {FavoriteId}", command.Id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating favorite {FavoriteId}", command.Id);
            throw;
        }
    }
}

public class DeleteFavoriteCommandHandler
{
    private readonly IFavoriteRepository _favoriteRepo;
    private readonly ILogger<DeleteFavoriteCommandHandler> _logger;

    public DeleteFavoriteCommandHandler(IFavoriteRepository favoriteRepo, ILogger<DeleteFavoriteCommandHandler> logger)
    {
        _favoriteRepo = favoriteRepo;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(DeleteFavoriteCommand command)
    {
        try
        {
            _logger.LogInformation("Deleting favorite {FavoriteId} for user {UserId}", command.Id, command.UserId);

            // Verify ownership before deleting
            var favorite = await _favoriteRepo.GetByIdAsync(command.Id).ConfigureAwait(false);
            if (favorite == null)
            {
                _logger.LogWarning("Favorite {FavoriteId} not found for deletion", command.Id);
                return false;
            }

            if (favorite.UserId != command.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to delete favorite {FavoriteId} owned by user {OwnerId}",
                    command.UserId, command.Id, favorite.UserId);
                throw new UnauthorizedAccessException("Cannot delete favorite owned by another user");
            }

            var result = await _favoriteRepo.DeleteAsync(command.Id).ConfigureAwait(false);

            if (result)
                _logger.LogInformation("Successfully deleted favorite {FavoriteId}", command.Id);
            else
                _logger.LogWarning("Failed to delete favorite {FavoriteId}", command.Id);

            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization errors
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting favorite {FavoriteId}", command.Id);
            throw;
        }
    }
}

public class GetFavoritesQueryHandler
{
    private readonly IFavoriteRepository _favoriteRepo;
    private readonly ILogger<GetFavoritesQueryHandler> _logger;

    public GetFavoritesQueryHandler(IFavoriteRepository favoriteRepo, ILogger<GetFavoritesQueryHandler> logger)
    {
        _favoriteRepo = favoriteRepo;
        _logger = logger;
    }

    public async Task<List<Favorite>> HandleAsync(GetFavoritesQuery query)
    {
        try
        {
            _logger.LogInformation("Getting favorites for user {UserId}", query.UserId);

            var favorites = await _favoriteRepo.GetByUserIdAsync(query.UserId).ConfigureAwait(false);
            var result = favorites.ToList();

            _logger.LogInformation("Found {Count} favorites for user {UserId}", result.Count, query.UserId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorites for user {UserId}", query.UserId);
            throw;
        }
    }
}
