// Application/Templates/Commands/CreateTemplateCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Templates.Commands;

public record CreateTemplateCommand(
    string Name,
    string Description,
    string Category,
    string YamlContent,
    int CreatedByUserId
);

public class CreateTemplateCommandHandler
{
    private readonly ITemplateRepository _templateRepo;
    private readonly IAuditLogRepository _auditRepo;

    public CreateTemplateCommandHandler(
        ITemplateRepository templateRepo,
        IAuditLogRepository auditRepo)
    {
        _templateRepo = templateRepo;
        _auditRepo = auditRepo;
    }

    public async Task<int> HandleAsync(CreateTemplateCommand command)
    {
        var template = new Template
        {
            Name = command.Name,
            Description = command.Description,
            Category = command.Category,
            // YamlContent stored in TemplateVersion, not Template
            CreatedBy = command.CreatedByUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var templateId = await _templateRepo.CreateAsync(template).ConfigureAwait(false);

        // Create initial version with YAML content
        var version = new TemplateVersion
        {
            TemplateId = templateId,
            VersionNumber = 1,
            YamlContent = command.YamlContent,
            IsCurrent = false,  // Will be set by SetCurrentVersionAsync
            CreatedBy = command.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };
        var versionId = await _templateRepo.CreateVersionAsync(version).ConfigureAwait(false);
        
        // Set the new version as current
        await _templateRepo.SetCurrentVersionAsync(templateId, versionId).ConfigureAwait(false);

        await _auditRepo.CreateAsync(new AuditLog
        {
            UserId = command.CreatedByUserId,
            Action = "Create",
            Success = true,
            ResourceKind = "Template",
            ResourceName = command.Name,
            CreatedAt = DateTime.UtcNow
        }).ConfigureAwait(false);

        return templateId;
    }
}
