// Application/Templates/Queries/GetTemplateByIdQuery.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Application.Templates.Queries;

public record GetTemplateByIdQuery(int Id);

public class GetTemplateByIdQueryHandler
{
    private readonly ITemplateRepository _templateRepo;

    public GetTemplateByIdQueryHandler(ITemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    public async Task<TemplateResponse?> HandleAsync(GetTemplateByIdQuery query)
    {
        var template = await _templateRepo.GetByIdAsync(query.Id, includeVersions: false).ConfigureAwait(false);
        if (template == null) return null;

        var currentVersion = await _templateRepo.GetCurrentVersionAsync(query.Id).ConfigureAwait(false);

        return new TemplateResponse(
            Id: template.Id,
            Name: template.Name,
            Category: template.Category ?? string.Empty,
            Description: template.Description,
            Tags: template.Tags,
            YamlContent: currentVersion?.YamlContent,
            Version: currentVersion?.VersionNumber ?? 1,
            IsPublic: template.IsPublic,
            CreatedBy: template.CreatedBy,
            CreatedAt: template.CreatedAt,
            UpdatedAt: template.UpdatedAt
        );
    }
}
