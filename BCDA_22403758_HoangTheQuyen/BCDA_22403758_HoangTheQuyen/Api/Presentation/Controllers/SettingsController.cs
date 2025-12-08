// Presentation/Controllers/SettingsController.cs
using System.Security.Claims;
using K8sManager.Api.Application.Settings;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly UpsertSettingCommandHandler _upsertHandler;
    private readonly DeleteSettingCommandHandler _deleteHandler;
    private readonly GetAllSettingsQueryHandler _getAllHandler;
    private readonly GetSettingByKeyQueryHandler _getByKeyHandler;
    private readonly GetSettingsByCategoryQueryHandler _getByCategoryHandler;

    public SettingsController(
        UpsertSettingCommandHandler upsertHandler,
        DeleteSettingCommandHandler deleteHandler,
        GetAllSettingsQueryHandler getAllHandler,
        GetSettingByKeyQueryHandler getByKeyHandler,
        GetSettingsByCategoryQueryHandler getByCategoryHandler)
    {
        _upsertHandler = upsertHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByKeyHandler = getByKeyHandler;
        _getByCategoryHandler = getByCategoryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<AppSetting>>> GetAll()
    {
        var query = new GetAllSettingsQuery();
        var settings = await _getAllHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(settings);
    }

    [HttpGet("key/{key}")]
    public async Task<ActionResult<AppSetting>> GetByKey(string key)
    {
        var query = new GetSettingByKeyQuery(key);
        var setting = await _getByKeyHandler.HandleAsync(query).ConfigureAwait(false);

        if (setting == null)
            return NotFound(new { Message = "Setting not found" });

        return Ok(setting);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<AppSetting>>> GetByCategory(string category)
    {
        var query = new GetSettingsByCategoryQuery(category);
        var settings = await _getByCategoryHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(settings);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Upsert([FromBody] UpsertSettingRequest request)
    {
        var userId = GetUserId();
        var command = new UpsertSettingCommand(
            request.Key,
            request.Value,
            request.Category,
            request.IsEncrypted,
            userId
        );

        var id = await _upsertHandler.HandleAsync(command).ConfigureAwait(false);
        return Ok(new { Id = id });
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string key)
    {
        var command = new DeleteSettingCommand(key);
        var result = await _deleteHandler.HandleAsync(command).ConfigureAwait(false);

        if (!result)
            return NotFound(new { Message = "Setting not found" });

        return NoContent();
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
