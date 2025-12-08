// Application/Templates/Commands/UpdateTemplateCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Templates.Commands;

public record UpdateTemplateCommand(
    int Id,
    string Name,
    string Description,
    string Category,
    string YamlContent,
    int UpdatedByUserId
);

public class UpdateTemplateCommandHandler
{
    private readonly ITemplateRepository _templateRepo;
    private readonly IAuditLogRepository _auditRepo;

    public UpdateTemplateCommandHandler(
        ITemplateRepository templateRepo,
        IAuditLogRepository auditRepo)
    {
        _templateRepo = templateRepo;
        _auditRepo = auditRepo;
    }

    public async Task<bool> HandleAsync(UpdateTemplateCommand command)
    {
        var existing = await _templateRepo.GetByIdAsync(command.Id).ConfigureAwait(false);
        if (existing == null) return false;

        existing.Name = command.Name;
        existing.Description = command.Description;
        existing.Category = command.Category;
        existing.UpdatedAt = DateTime.UtcNow;
        // YamlContent update creates new version, not stored on Template

        var result = await _templateRepo.UpdateAsync(existing).ConfigureAwait(false);

        if (result)
        {
            // Create new version if YAML changed
            var versions = await _templateRepo.GetVersionsByTemplateIdAsync(command.Id).ConfigureAwait(false);
            var maxVersion = versions.Any() ? versions.Max(v => v.VersionNumber) : 0;
            
            var newVersion = new TemplateVersion
            {
                TemplateId = command.Id,
                VersionNumber = maxVersion + 1,
                YamlContent = command.YamlContent,
                IsCurrent = false,  // Will be set by SetCurrentVersionAsync
                CreatedBy = command.UpdatedByUserId,
                CreatedAt = DateTime.UtcNow
            };
            var newVersionId = await _templateRepo.CreateVersionAsync(newVersion).ConfigureAwait(false);
            
            // Set the new version as current (this will unset all other versions)
            await _templateRepo.SetCurrentVersionAsync(command.Id, newVersionId).ConfigureAwait(false);

            await _auditRepo.CreateAsync(new AuditLog
            {
                UserId = command.UpdatedByUserId,
                Action = "Update",
                Success = true,
                ResourceKind = "Template",
                ResourceName = command.Name,
                CreatedAt = DateTime.UtcNow
            }).ConfigureAwait(false);
        }

        return result;
    }
}
