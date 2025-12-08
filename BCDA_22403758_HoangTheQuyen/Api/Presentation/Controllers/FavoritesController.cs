// Presentation/Controllers/FavoritesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Application.Favorites;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly CreateFavoriteCommandHandler _createHandler;
    private readonly UpdateFavoriteCommandHandler _updateHandler;
    private readonly DeleteFavoriteCommandHandler _deleteHandler;
    private readonly GetFavoritesQueryHandler _getFavoritesHandler;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(
        CreateFavoriteCommandHandler createHandler,
        UpdateFavoriteCommandHandler updateHandler,
        DeleteFavoriteCommandHandler deleteHandler,
        GetFavoritesQueryHandler getFavoritesHandler,
        ILogger<FavoritesController> logger)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getFavoritesHandler = getFavoritesHandler;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Favorite>>> GetMyFavorites()
    {
        try
        {
            var userId = GetUserId();
            var query = new GetFavoritesQuery(userId);
            var favorites = await _getFavoritesHandler.HandleAsync(query).ConfigureAwait(false);
            return Ok(favorites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorites");
            return StatusCode(500, new { Message = "Error retrieving favorites", Error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateFavoriteRequest request)
    {
        try
        {
            var userId = GetUserId();
            var command = new CreateFavoriteCommand(
                userId,
                request.ResourceType,
                request.ResourceName,
                request.ClusterName,
                request.Namespace,
                request.Notes
            );

            var id = await _createHandler.HandleAsync(command).ConfigureAwait(false);
            return Ok(new { Id = id });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating favorite");
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating favorite");
            return StatusCode(500, new { Message = "Error creating favorite", Error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateFavoriteRequest request)
    {
        try
        {
            var command = new UpdateFavoriteCommand(id, request.Notes);
            var result = await _updateHandler.HandleAsync(command).ConfigureAwait(false);

            if (!result)
                return NotFound(new { Message = $"Favorite with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating favorite {FavoriteId}", id);
            return StatusCode(500, new { Message = "Error updating favorite", Error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var userId = GetUserId();
            var command = new DeleteFavoriteCommand(id, userId);
            var result = await _deleteHandler.HandleAsync(command).ConfigureAwait(false);

            if (!result)
                return NotFound(new { Message = $"Favorite with ID {id} not found" });

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized attempt to delete favorite {FavoriteId}", id);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting favorite {FavoriteId}", id);
            return StatusCode(500, new { Message = "Error deleting favorite", Error = ex.Message });
        }
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
