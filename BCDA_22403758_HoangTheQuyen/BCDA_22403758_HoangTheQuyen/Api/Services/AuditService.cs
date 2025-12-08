using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repo;
        public AuditService(IAuditRepository repo) => _repo = repo;

        public Task LogAsync(int userId, string action, string? kind, string? name, string? ns, bool success, string? msg)
            => _repo.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = action,
                ResourceKind = kind,
                ResourceName = name,
                Namespace = ns,
                Success = success,
                ErrorMessage = msg
            });
    }
}