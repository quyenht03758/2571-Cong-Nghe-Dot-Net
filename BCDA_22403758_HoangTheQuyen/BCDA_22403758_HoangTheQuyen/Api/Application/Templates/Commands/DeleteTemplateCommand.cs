// Application/Templates/Commands/DeleteTemplateCommand.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Templates.Commands;

public record DeleteTemplateCommand(int Id, int DeletedByUserId);

public class DeleteTemplateCommandHandler
{
    private readonly ITemplateRepository _templateRepo;
    private readonly IAuditLogRepository _auditRepo;

    public DeleteTemplateCommandHandler(
        ITemplateRepository templateRepo,
        IAuditLogRepository auditRepo)
    {
        _templateRepo = templateRepo;
        _auditRepo = auditRepo;
    }

    public async Task<bool> HandleAsync(DeleteTemplateCommand command)
    {
        var template = await _templateRepo.GetByIdAsync(command.Id).ConfigureAwait(false);
        if (template == null) return false;

        var result = await _templateRepo.DeleteAsync(command.Id).ConfigureAwait(false);

        if (result)
        {
            await _auditRepo.CreateAsync(new AuditLog
            {
                UserId = command.DeletedByUserId,
                Action = "Delete",
                Success = true,
                ResourceKind = "Template",
                ResourceName = template.Name,
                CreatedAt = DateTime.UtcNow
            }).ConfigureAwait(false);
        }

        return result;
    }
}
