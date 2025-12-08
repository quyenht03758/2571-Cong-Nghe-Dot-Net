// Application/Templates/Commands/CreateTemplateVersionCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Templates.Commands;

public record CreateTemplateVersionCommand(
    int TemplateId,
    int Version,
    string YamlContent,
    string? ChangeLog,
    int CreatedByUserId
);

public class CreateTemplateVersionCommandHandler
{
    private readonly ITemplateRepository _templateRepo;
    private readonly IAuditLogRepository _auditRepo;

    public CreateTemplateVersionCommandHandler(
        ITemplateRepository templateRepo,
        IAuditLogRepository auditRepo)
    {
        _templateRepo = templateRepo;
        _auditRepo = auditRepo;
    }

    public async Task<int> HandleAsync(CreateTemplateVersionCommand command)
    {
        // Validate template exists
        var template = await _templateRepo.GetByIdAsync(command.TemplateId).ConfigureAwait(false);
        if (template == null)
        {
            throw new InvalidOperationException($"Template {command.TemplateId} not found");
        }

        // Determine version number
        var versionNumber = command.Version;
        if (versionNumber <= 0)
        {
            // Auto-calculate next version number
            var existingVersions = await _templateRepo.GetVersionsByTemplateIdAsync(command.TemplateId).ConfigureAwait(false);
            versionNumber = existingVersions.Any() ? existingVersions.Max(v => v.VersionNumber) + 1 : 1;
        }
        else
        {
            // Check if version already exists
            var existingVersions = await _templateRepo.GetVersionsByTemplateIdAsync(command.TemplateId).ConfigureAwait(false);
            if (existingVersions.Any(v => v.VersionNumber == versionNumber))
            {
                throw new InvalidOperationException($"Version {versionNumber} already exists for template {command.TemplateId}");
            }
        }

        var templateVersion = new TemplateVersion
        {
            TemplateId = command.TemplateId,
            VersionNumber = versionNumber,
            YamlContent = command.YamlContent,
            ChangeLog = command.ChangeLog ?? $"Version {versionNumber}",
            IsCurrent = false,  // Will be set explicitly if needed
            CreatedBy = command.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        var versionId = await _templateRepo.CreateVersionAsync(templateVersion).ConfigureAwait(false);
        
        // Set this new version as current (this will unset all other versions)
        await _templateRepo.SetCurrentVersionAsync(command.TemplateId, versionId).ConfigureAwait(false);

        await _auditRepo.CreateAsync(new AuditLog
        {
            UserId = command.CreatedByUserId,
            Action = "CreateVersion",
            ResourceKind = "TemplateVersion",
            ResourceName = $"Template {command.TemplateId} v{versionNumber}",
            Success = true,
            CreatedAt = DateTime.UtcNow
        }).ConfigureAwait(false);

        return versionId;
    }
}
