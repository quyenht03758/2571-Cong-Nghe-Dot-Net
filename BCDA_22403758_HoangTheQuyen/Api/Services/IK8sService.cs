using k8s.Models;

namespace K8sManager.Api.Services;

public interface IK8sService
{
    // Namespaces
    Task<IList<V1Namespace>> ListNamespacesAsync(string? kubeconfigPath = null, string? contextName = null);
    Task<V1Namespace> CreateNamespaceAsync(string name, string? kubeconfigPath = null, string? contextName = null);

    // Pods
    Task<IList<V1Pod>> ListPodsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null);
    Task<V1Pod?> GetPodAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
    Task<string> GetPodLogsAsync(string @namespace, string name, string? container = null, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> DeletePodAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);

    // Deployments
    Task<IList<V1Deployment>> ListDeploymentsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> ScaleDeploymentAsync(string @namespace, string name, int replicas, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> DeleteDeploymentAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);

    // Services
    Task<IList<V1Service>> ListServicesAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> DeleteServiceAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);

    // ConfigMaps
    Task<IList<V1ConfigMap>> ListConfigMapsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> DeleteConfigMapAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);

    // Secrets
    Task<IList<V1Secret>> ListSecretsAsync(string @namespace, string? kubeconfigPath = null, string? contextName = null);
    Task<bool> DeleteSecretAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);

    // YAML Apply
    Task<(bool Success, string Message)> ApplyYamlAsync(string yamlContent, string? kubeconfigPath = null, string? contextName = null);

    // YAML Export
    Task<string> GetPodYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
    Task<string> GetDeploymentYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
    Task<string> GetServiceYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
    Task<string> GetConfigMapYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
    Task<string> GetSecretYamlAsync(string @namespace, string name, string? kubeconfigPath = null, string? contextName = null);
}
