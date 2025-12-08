using k8s;
using k8s.Models;
using K8sManager.Shared.Infrastructure;

namespace K8sManager.Shared.Services;

public class K8sService : IK8sService
{
    private readonly K8sClientFactory _factory;

    public K8sService(K8sClientFactory factory) => _factory = factory;

    public async Task<IList<string>> ListNamespacesAsync()
    {
        using var client = _factory.Create();
        var nsList = await client.CoreV1.ListNamespaceAsync().ConfigureAwait(false);
        return nsList.Items
            .Select(n => n.Metadata?.Name ?? "")
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .OrderBy(n => n)
            .ToList();
    }

    public async Task<IList<V1Pod>> ListPodsAsync(string ns)
    {
        using var client = _factory.Create();
        var pods = await client.CoreV1.ListNamespacedPodAsync(namespaceParameter: ns).ConfigureAwait(false);
        return pods.Items;
    }

    public async Task<bool> ScaleDeploymentAsync(string ns, string name, int replicas)
    {
        try
        {
            using var client = _factory.Create();

            var scale = new V1Scale
            {
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    NamespaceProperty = ns
                },
                Spec = new V1ScaleSpec
                {
                    Replicas = replicas
                }
            };

            await client.AppsV1.ReplaceNamespacedDeploymentScaleAsync(
                body: scale,
                name: name,
                namespaceParameter: ns
            ).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ScaleDeploymentAsync] {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeletePodAsync(string ns, string name)
    {
        try
        {
            using var client = _factory.Create();
            var opts = new V1DeleteOptions();
            await client.CoreV1.DeleteNamespacedPodAsync(
                name: name,
                namespaceParameter: ns,
                body: opts
            ).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DeletePodAsync] {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    public async IAsyncEnumerable<string> TailLogsAsync(
        string ns,
        string pod,
        string? container = null,
        int tailLines = 200)
    {
        using var client = _factory.Create();
        using var stream = await client.CoreV1.ReadNamespacedPodLogAsync(
            name: pod,
            namespaceParameter: ns,
            container: container,
            follow: true,
            tailLines: tailLines
        ).ConfigureAwait(false);

        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line is not null) yield return line;
        }
    }

    public async Task<string> GetServerVersionAsync()
    {
        using var client = _factory.Create();
        var ver = await client.Version.GetCodeAsync().ConfigureAwait(false);
        return ver?.GitVersion ?? "(unknown)";
    }
}
