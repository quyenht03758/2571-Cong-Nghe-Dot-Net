using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/clusters")]
[Authorize]
public class ClustersController : ControllerBase
{
    private readonly IClusterRepository _clusterRepo;

    public ClustersController(IClusterRepository clusterRepo)
    {
        _clusterRepo = clusterRepo;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClusterDto>>> GetAll()
    {
        var clusters = await _clusterRepo.GetAllAsync().ConfigureAwait(false);
        var dtos = clusters.Select(c => new ClusterDto(
            c.Id,
            c.Name,
            c.KubeconfigPath,
            c.ContextName,
            c.IsDefault,
            c.Environment,
            c.Description,
            c.CreatedAt
        )).ToList();
        
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClusterDto>> GetById(int id)
    {
        var cluster = await _clusterRepo.GetByIdAsync(id).ConfigureAwait(false);
        
        if (cluster == null)
            return NotFound(new { Message = "Cluster not found" });

        var dto = new ClusterDto(
            cluster.Id,
            cluster.Name,
            cluster.KubeconfigPath,
            cluster.ContextName,
            cluster.IsDefault,
            cluster.Environment,
            cluster.Description,
            cluster.CreatedAt
        );
        
        return Ok(dto);
    }

    [HttpGet("default")]
    public async Task<ActionResult<ClusterDto>> GetDefault()
    {
        var cluster = await _clusterRepo.GetDefaultAsync().ConfigureAwait(false);
        
        if (cluster == null)
            return NotFound(new { Message = "No default cluster configured" });

        var dto = new ClusterDto(
            cluster.Id,
            cluster.Name,
            cluster.KubeconfigPath,
            cluster.ContextName,
            cluster.IsDefault,
            cluster.Environment,
            cluster.Description,
            cluster.CreatedAt
        );
        
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ClusterDto>> Create([FromBody] CreateClusterRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int userId = userIdClaim != null && int.TryParse(userIdClaim, out int uid) ? uid : 1;

        var cluster = new ClusterConfig
        {
            Name = request.Name,
            KubeconfigPath = request.KubeconfigPath,
            ContextName = request.ContextName ?? "",
            IsDefault = request.IsDefault,
            Environment = request.Environment,
            Description = request.Description,
            AddedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _clusterRepo.AddAsync(cluster).ConfigureAwait(false);
        cluster.Id = id;

        var dto = new ClusterDto(
            cluster.Id,
            cluster.Name,
            cluster.KubeconfigPath,
            cluster.ContextName,
            cluster.IsDefault,
            cluster.Environment,
            cluster.Description,
            cluster.CreatedAt
        );
        
        return CreatedAtAction(nameof(GetById), new { id }, dto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var cluster = await _clusterRepo.GetByIdAsync(id).ConfigureAwait(false);
        
        if (cluster == null)
            return NotFound(new { Message = "Cluster not found" });

        await _clusterRepo.DeleteAsync(id).ConfigureAwait(false);
        
        return NoContent();
    }

    [HttpPost("{id}/set-default")]
    public async Task<ActionResult> SetDefault(int id)
    {
        var cluster = await _clusterRepo.GetByIdAsync(id).ConfigureAwait(false);
        
        if (cluster == null)
            return NotFound(new { Message = "Cluster not found" });

        await _clusterRepo.SetDefaultAsync(id).ConfigureAwait(false);
        
        return Ok(new { Message = "Default cluster updated successfully" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateClusterRequest request)
    {
        var cluster = await _clusterRepo.GetByIdAsync(id).ConfigureAwait(false);
        
        if (cluster == null)
            return NotFound(new { Message = "Cluster not found" });

        cluster.Name = request.Name;
        cluster.KubeconfigPath = request.KubeconfigPath;
        cluster.ContextName = request.ContextName ?? "";
        cluster.IsDefault = request.IsDefault;
        cluster.Environment = request.Environment;
        cluster.Description = request.Description;
        cluster.UpdatedAt = DateTime.UtcNow;

        await _clusterRepo.UpdateAsync(cluster).ConfigureAwait(false);
        
        return NoContent();
    }
}
