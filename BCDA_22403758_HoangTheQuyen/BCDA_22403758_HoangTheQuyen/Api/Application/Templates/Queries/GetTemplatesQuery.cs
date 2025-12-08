// Application/Templates/Queries/GetTemplatesQuery.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Application.Templates.Queries;

public record GetTemplatesQuery(
    string? Category = null,
    string? SearchTerm = null
);

public class GetTemplatesQueryHandler
{
    private readonly ITemplateRepository _templateRepo;

    public GetTemplatesQueryHandler(ITemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    public async Task<List<TemplateResponse>> HandleAsync(GetTemplatesQuery query)
    {
        var templatesWithVersions = await _templateRepo.GetAllWithCurrentVersionAsync(query.Category, query.SearchTerm).ConfigureAwait(false);
        var result = new List<TemplateResponse>();

        foreach (var (template, currentVersion) in templatesWithVersions)
        {
            result.Add(new TemplateResponse(
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
            ));
        }

        return result;
    }
}
