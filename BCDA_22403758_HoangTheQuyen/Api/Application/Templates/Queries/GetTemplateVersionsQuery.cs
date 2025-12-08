// Api/Application/Templates/Queries/GetTemplateVersionsQuery.cs
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.DTOs;

namespace K8sManager.Api.Application.Templates.Queries;

public record GetTemplateVersionsQuery(int TemplateId);

public class GetTemplateVersionsQueryHandler
{
    private readonly ITemplateRepository _templateRepo;

    public GetTemplateVersionsQueryHandler(ITemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    public async Task<List<TemplateVersionResponse>> HandleAsync(GetTemplateVersionsQuery query)
    {
        var versions = await _templateRepo.GetVersionsByTemplateIdAsync(query.TemplateId).ConfigureAwait(false);
        
        return versions.Select(v => new TemplateVersionResponse(
            Id: v.Id,
            TemplateId: v.TemplateId,
            VersionNumber: v.VersionNumber,
            YamlContent: v.YamlContent,
            ChangeLog: v.ChangeLog,
            IsCurrent: v.IsCurrent,
            CreatedBy: v.CreatedBy,
            CreatedAt: v.CreatedAt
        )).ToList();
    }
}
