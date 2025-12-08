using k8s;
using k8s.Models;
using K8sManager.Api.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace K8sManager.Api.Infrastructure.K8s;

public class K8sServiceImplementation : IK8sService
{
    private readonly K8sClientFactory _defaultFactory;

    public K8sServiceImplementation(K8sClientFactory factory)
    {
        _defaultFactory = factory;
    }

    private IKubernetes CreateClient(string? kubeconfigPath = null, string? contextName = null)
    {
        if (!string.IsNullOrEmpty(kubeconfigPath))
        {
            // Create new factory with specified config
            var factory = new K8sClientFactory(kubeconfigPath, contextName);
            return factory.Create();
        }
        
        // Use default factory
        if (!string.IsNullOrEmpty(contextName))
        {
            _defaultFactory.SetContext(contextName);
        }
        return _defaultFactory.Create();
    }

    // Namespaces
    public async Task<IList<V1Namespace>> ListNamespacesAsync(string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.CoreV1.ListNamespaceAsync().ConfigureAwait(false);
        return list.Items;
    }

    public async Task<V1Namespace> CreateNamespaceAsync(string name, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var ns = new V1Namespace
        {
            Metadata = new V1ObjectMeta { Name = name }
        };
        return await client.CoreV1.CreateNamespaceAsync(ns).ConfigureAwait(false);
    }

    // Pods
    public async Task<IList<V1Pod>> ListPodsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.CoreV1.ListNamespacedPodAsync(@namespace).ConfigureAwait(false);
        return list.Items;
    }

    public async Task<V1Pod?> GetPodAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            return await client.CoreV1.ReadNamespacedPodAsync(name, @namespace).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GetPodLogsAsync(string @namespace, string name, string? container = null, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        using var stream = await client.CoreV1.ReadNamespacedPodLogAsync(name, @namespace, container: container).ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public async Task<bool> DeletePodAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            await client.CoreV1.DeleteNamespacedPodAsync(name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Deployments
    public async Task<IList<V1Deployment>> ListDeploymentsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.AppsV1.ListNamespacedDeploymentAsync(@namespace).ConfigureAwait(false);
        return list.Items;
    }

    public async Task<bool> ScaleDeploymentAsync(string @namespace, string name, int replicas, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            var scale = new V1Scale
            {
                Metadata = new V1ObjectMeta { Name = name, NamespaceProperty = @namespace },
                Spec = new V1ScaleSpec { Replicas = replicas }
            };
            await client.AppsV1.ReplaceNamespacedDeploymentScaleAsync(scale, name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteDeploymentAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            await client.AppsV1.DeleteNamespacedDeploymentAsync(name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Services
    public async Task<IList<V1Service>> ListServicesAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.CoreV1.ListNamespacedServiceAsync(@namespace).ConfigureAwait(false);
        return list.Items;
    }

    public async Task<bool> DeleteServiceAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            await client.CoreV1.DeleteNamespacedServiceAsync(name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ConfigMaps
    public async Task<IList<V1ConfigMap>> ListConfigMapsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.CoreV1.ListNamespacedConfigMapAsync(@namespace).ConfigureAwait(false);
        return list.Items;
    }

    public async Task<bool> DeleteConfigMapAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            await client.CoreV1.DeleteNamespacedConfigMapAsync(name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Secrets
    public async Task<IList<V1Secret>> ListSecretsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var list = await client.CoreV1.ListNamespacedSecretAsync(@namespace).ConfigureAwait(false);
        return list.Items;
    }

    public async Task<bool> DeleteSecretAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            await client.CoreV1.DeleteNamespacedSecretAsync(name, @namespace).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // YAML Apply
    public async Task<(bool Success, string Message)> ApplyYamlAsync(string yamlContent, string? kubeconfigPath = null, string? contextName = null)
    {
        try
        {
            using var client = CreateClient(kubeconfigPath, contextName);
            
            if (string.IsNullOrWhiteSpace(yamlContent))
            {
                return (false, "YAML content is empty");
            }

            List<object> objects;
            try
            {
                objects = KubernetesYaml.LoadAllFromString(yamlContent).ToList();
            }
            catch (Exception ex)
            {
                return (false, $"Invalid YAML format: {ex.Message}");
            }

            if (objects.Count == 0)
            {
                return (false, "No Kubernetes objects found in YAML");
            }

            var results = new List<string>();
            
            foreach (var obj in objects)
            {
                try
                {
                    var result = await ApplyObjectAsync(client, obj).ConfigureAwait(false);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add($"Failed to apply {obj.GetType().Name}: {ex.Message}");
                }
            }
            
            return (true, string.Join("\n", results));
        }
        catch (Exception ex)
        {
            return (false, $"Failed to apply YAML: {ex.Message}");
        }
    }

    private async Task<string> ApplyObjectAsync(IKubernetes client, object obj)
    {
        var ns = "default";
        var name = "";

        try
        {
            if (obj is V1Namespace nsObj)
            {
                name = nsObj.Metadata?.Name ?? "unknown";
                try
                {
                    await client.CoreV1.CreateNamespaceAsync(nsObj).ConfigureAwait(false);
                    return $"✓ Created namespace: {name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return $"⚠ Namespace already exists: {name}";
                }
            }
            else if (obj is V1Pod pod)
            {
                ns = pod.Metadata?.NamespaceProperty ?? "default";
                name = pod.Metadata?.Name ?? "unknown";
                try
                {
                    await client.CoreV1.CreateNamespacedPodAsync(pod, ns).ConfigureAwait(false);
                    return $"✓ Created pod: {ns}/{name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return $"⚠ Pod already exists: {ns}/{name}";
                }
            }
            else if (obj is V1Deployment deploy)
            {
                ns = deploy.Metadata?.NamespaceProperty ?? "default";
                name = deploy.Metadata?.Name ?? "unknown";
                try
                {
                    await client.AppsV1.CreateNamespacedDeploymentAsync(deploy, ns).ConfigureAwait(false);
                    return $"✓ Created deployment: {ns}/{name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await client.AppsV1.ReplaceNamespacedDeploymentAsync(deploy, name, ns).ConfigureAwait(false);
                    return $"✓ Updated deployment: {ns}/{name}";
                }
            }
            else if (obj is V1Service svc)
            {
                ns = svc.Metadata?.NamespaceProperty ?? "default";
                name = svc.Metadata?.Name ?? "unknown";
                try
                {
                    await client.CoreV1.CreateNamespacedServiceAsync(svc, ns).ConfigureAwait(false);
                    return $"✓ Created service: {ns}/{name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // Services need special handling for clusterIP
                    var existing = await client.CoreV1.ReadNamespacedServiceAsync(name, ns).ConfigureAwait(false);
                    svc.Spec.ClusterIP = existing.Spec.ClusterIP;
                    if (existing.Metadata?.ResourceVersion != null && svc.Metadata != null)
                    {
                        svc.Metadata.ResourceVersion = existing.Metadata.ResourceVersion;
                    }
                    await client.CoreV1.ReplaceNamespacedServiceAsync(svc, name, ns).ConfigureAwait(false);
                    return $"✓ Updated service: {ns}/{name}";
                }
            }
            else if (obj is V1ConfigMap cm)
            {
                ns = cm.Metadata?.NamespaceProperty ?? "default";
                name = cm.Metadata?.Name ?? "unknown";
                try
                {
                    await client.CoreV1.CreateNamespacedConfigMapAsync(cm, ns).ConfigureAwait(false);
                    return $"✓ Created configmap: {ns}/{name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await client.CoreV1.ReplaceNamespacedConfigMapAsync(cm, name, ns).ConfigureAwait(false);
                    return $"✓ Updated configmap: {ns}/{name}";
                }
            }
            else if (obj is V1Secret secret)
            {
                ns = secret.Metadata?.NamespaceProperty ?? "default";
                name = secret.Metadata?.Name ?? "unknown";
                try
                {
                    await client.CoreV1.CreateNamespacedSecretAsync(secret, ns).ConfigureAwait(false);
                    return $"✓ Created secret: {ns}/{name}";
                }
                catch (k8s.Autorest.HttpOperationException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    await client.CoreV1.ReplaceNamespacedSecretAsync(secret, name, ns).ConfigureAwait(false);
                    return $"✓ Updated secret: {ns}/{name}";
                }
            }
            else
            {
                return $"⚠ Unsupported resource type: {obj.GetType().Name}";
            }
        }
        catch (Exception ex)
        {
            return $"✗ Error applying {obj.GetType().Name} {ns}/{name}: {ex.Message}";
        }
    }

    // YAML Export
    public async Task<string> GetPodYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        var pod = await GetPodAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
        return pod != null ? SerializeToYaml(pod) : string.Empty;
    }

    public async Task<string> GetDeploymentYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(name, @namespace).ConfigureAwait(false);
        return SerializeToYaml(deployment);
    }

    public async Task<string> GetServiceYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var service = await client.CoreV1.ReadNamespacedServiceAsync(name, @namespace).ConfigureAwait(false);
        return SerializeToYaml(service);
    }

    public async Task<string> GetConfigMapYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var configMap = await client.CoreV1.ReadNamespacedConfigMapAsync(name, @namespace).ConfigureAwait(false);
        return SerializeToYaml(configMap);
    }

    public async Task<string> GetSecretYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null)
    {
        using var client = CreateClient(kubeconfigPath, contextName);
        var secret = await client.CoreV1.ReadNamespacedSecretAsync(name, @namespace).ConfigureAwait(false);
        return SerializeToYaml(secret);
    }

    private string SerializeToYaml(object obj)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Serialize(obj);
    }
}
