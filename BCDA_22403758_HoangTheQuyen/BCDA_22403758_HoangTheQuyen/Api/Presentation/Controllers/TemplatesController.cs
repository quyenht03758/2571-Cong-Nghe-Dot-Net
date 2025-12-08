// Presentation/Controllers/TemplatesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Application.Templates.Commands;
using K8sManager.Api.Application.Templates.Queries;
using K8sManager.Api.DTOs;
using K8sManager.Api.Domain.Entities;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly CreateTemplateCommandHandler _createHandler;
    private readonly UpdateTemplateCommandHandler _updateHandler;
    private readonly DeleteTemplateCommandHandler _deleteHandler;
    private readonly CreateTemplateVersionCommandHandler _createVersionHandler;
    private readonly GetTemplatesQueryHandler _getTemplatesHandler;
    private readonly GetTemplateByIdQueryHandler _getByIdHandler;
    private readonly GetTemplateVersionsQueryHandler _getVersionsHandler;

    public TemplatesController(
        CreateTemplateCommandHandler createHandler,
        UpdateTemplateCommandHandler updateHandler,
        DeleteTemplateCommandHandler deleteHandler,
        CreateTemplateVersionCommandHandler createVersionHandler,
        GetTemplatesQueryHandler getTemplatesHandler,
        GetTemplateByIdQueryHandler getByIdHandler,
        GetTemplateVersionsQueryHandler getVersionsHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _createVersionHandler = createVersionHandler;
        _getTemplatesHandler = getTemplatesHandler;
        _getByIdHandler = getByIdHandler;
        _getVersionsHandler = getVersionsHandler;
    }

    [HttpGet]
    public async Task<ActionResult<List<TemplateResponse>>> GetAll(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null)
    {
        var query = new GetTemplatesQuery(category, search);
        var templates = await _getTemplatesHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateResponse>> GetById(int id)
    {
        var query = new GetTemplateByIdQuery(id);
        var template = await _getByIdHandler.HandleAsync(query).ConfigureAwait(false);

        if (template == null)
            return NotFound(new { Message = "Template not found" });

        return Ok(template);
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult<List<TemplateVersionResponse>>> GetVersions(int id)
    {
        var query = new GetTemplateVersionsQuery(id);
        var versions = await _getVersionsHandler.HandleAsync(query).ConfigureAwait(false);
        return Ok(versions);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<int>> Create([FromBody] CreateTemplateRequest request)
    {
        var userId = GetUserId();
        var command = new CreateTemplateCommand(
            request.Name,
            request.Description,
            request.Category,
            request.YamlContent,
            userId
        );

        var id = await _createHandler.HandleAsync(command).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTemplateRequest request)
    {
        var userId = GetUserId();
        var command = new UpdateTemplateCommand(
            id,
            request.Name,
            request.Description,
            request.Category,
            request.YamlContent,
            userId
        );

        var result = await _updateHandler.HandleAsync(command).ConfigureAwait(false);
        if (!result)
            return NotFound(new { Message = "Template not found" });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var command = new DeleteTemplateCommand(id, userId);

        var result = await _deleteHandler.HandleAsync(command).ConfigureAwait(false);
        if (!result)
            return NotFound(new { Message = "Template not found" });

        return NoContent();
    }

    [HttpPost("{id}/versions")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<ActionResult<int>> CreateVersion(int id, [FromBody] CreateTemplateVersionRequest request)
    {
        var userId = GetUserId();
        var command = new CreateTemplateVersionCommand(
            id,
            request.Version,
            request.YamlContent,
            request.ChangeLog,
            userId
        );

        var versionId = await _createVersionHandler.HandleAsync(command).ConfigureAwait(false);
        return Ok(new { Id = versionId });
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
