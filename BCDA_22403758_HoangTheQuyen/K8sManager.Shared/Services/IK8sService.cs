using k8s.Models;

namespace K8sManager.Shared.Services;

public interface IK8sService
{
    Task<IList<string>> ListNamespacesAsync();
    Task<IList<V1Pod>> ListPodsAsync(string ns);
    Task<bool> ScaleDeploymentAsync(string ns, string name, int replicas);
    Task<bool> DeletePodAsync(string ns, string name);
    IAsyncEnumerable<string> TailLogsAsync(string ns, string pod, string? container = null, int tailLines = 200);
    Task<string> GetServerVersionAsync();
}

public interface IAuditService
{
    Task LogAsync(int userId, string action, string? kind, string? name, string? ns, bool success, string? msg);
}

public interface ITemplateService
{
    Task<IEnumerable<(int Id, string Name)>> ListAsync();
}
