using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using K8sManager.Api.Services;
using K8sManager.Api.Domain.Repositories;
using K8sManager.Api.DTOs;
using K8sManager.Api.Infrastructure;

namespace K8sManager.Api.Presentation.Controllers;

[ApiController]
[Route("api/k8s")]
[Authorize]
public class K8sResourcesController : ControllerBase
{
    private readonly IK8sService _k8sService;
    private readonly IClusterRepository _clusterRepo;

    public K8sResourcesController(IK8sService k8sService, IClusterRepository clusterRepo)
    {
        _k8sService = k8sService;
        _clusterRepo = clusterRepo;
    }

    private async Task<(string? KubeconfigPath, string? ContextName)> GetClusterConfigAsync(int? clusterId)
    {
        if (!clusterId.HasValue)
            return (null, null);

        var cluster = await _clusterRepo.GetByIdAsync(clusterId.Value).ConfigureAwait(false);
        if (cluster == null)
            throw new InvalidOperationException($"Cluster with ID {clusterId} not found");

        return (cluster.KubeconfigPath, cluster.ContextName);
    }

    #region Contexts

    [HttpGet("contexts")]
    public async Task<ActionResult<List<string>>> GetContexts([FromQuery] string? kubeconfigPath = null)
    {
        try
        {
            // Use provided path or default kubeconfig
            var configPath = kubeconfigPath ?? "%USERPROFILE%\\.kube\\config";
            var factory = new K8sClientFactory(configPath);
            var contexts = factory.GetAvailableContexts();

            return Ok(contexts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving contexts: {ex.Message}" });
        }
    }

    #endregion

    #region Namespaces

    [HttpGet("namespaces")]
    public async Task<ActionResult<List<K8sNamespaceDto>>> GetNamespaces([FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var namespaces = await _k8sService.ListNamespacesAsync(kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = namespaces.Select(ns => new K8sNamespaceDto(
                ns.Metadata?.Name ?? "",
                ns.Status?.Phase ?? "",
                ns.Metadata?.CreationTimestamp ?? DateTime.MinValue,
                ns.Metadata?.Labels != null ? new Dictionary<string, string>(ns.Metadata.Labels) : new Dictionary<string, string>()
            )).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving namespaces: {ex.Message}" });
        }
    }

    [HttpPost("namespaces")]
    public async Task<ActionResult> CreateNamespace([FromBody] CreateNamespaceRequest request, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            await _k8sService.CreateNamespaceAsync(request.Name, kubeconfigPath, contextName).ConfigureAwait(false);

            return Ok(new { Success = true, Message = $"Namespace '{request.Name}' created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Success = false, Message = $"Error creating namespace: {ex.Message}" });
        }
    }

    #endregion

    #region Pods

    [HttpGet("pods")]
    public async Task<ActionResult<List<K8sPodDto>>> GetPods([FromQuery] string @namespace, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var pods = await _k8sService.ListPodsAsync(@namespace, kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = pods.Select(pod =>
            {
                var containers = pod.Spec?.Containers?.Select(c => new K8sContainerDto(
                    c.Name,
                    c.Image,
                    pod.Status?.ContainerStatuses?.FirstOrDefault(cs => cs.Name == c.Name)?.Ready ?? false,
                    pod.Status?.ContainerStatuses?.FirstOrDefault(cs => cs.Name == c.Name)?.RestartCount ?? 0
                )).ToList() ?? new List<K8sContainerDto>();

                return new K8sPodDto(
                    pod.Metadata?.Name ?? "",
                    pod.Metadata?.NamespaceProperty ?? "",
                    pod.Status?.Phase ?? "",
                    pod.Status?.Phase ?? "Unknown",
                    pod.Status?.ContainerStatuses?.Sum(cs => cs.RestartCount) ?? 0,
                    pod.Status?.PodIP ?? "",
                    pod.Spec?.NodeName ?? "",
                    pod.Metadata?.CreationTimestamp ?? DateTime.MinValue,
                    pod.Metadata?.Labels != null ? new Dictionary<string, string>(pod.Metadata.Labels) : new Dictionary<string, string>(),
                    containers
                );
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving pods: {ex.Message}" });
        }
    }

    [HttpGet("pods/{namespace}/{name}")]
    public async Task<ActionResult<K8sPodDto>> GetPod(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var pod = await _k8sService.GetPodAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (pod == null)
                return NotFound(new { Message = "Pod not found" });

            var containers = pod.Spec?.Containers?.Select(c => new K8sContainerDto(
                c.Name,
                c.Image,
                pod.Status?.ContainerStatuses?.FirstOrDefault(cs => cs.Name == c.Name)?.Ready ?? false,
                pod.Status?.ContainerStatuses?.FirstOrDefault(cs => cs.Name == c.Name)?.RestartCount ?? 0
            )).ToList() ?? new List<K8sContainerDto>();

            var dto = new K8sPodDto(
                pod.Metadata?.Name ?? "",
                pod.Metadata?.NamespaceProperty ?? "",
                pod.Status?.Phase ?? "",
                pod.Status?.Phase ?? "Unknown",
                pod.Status?.ContainerStatuses?.Sum(cs => cs.RestartCount) ?? 0,
                pod.Status?.PodIP ?? "",
                pod.Spec?.NodeName ?? "",
                pod.Metadata?.CreationTimestamp ?? DateTime.MinValue,
                pod.Metadata?.Labels != null ? new Dictionary<string, string>(pod.Metadata.Labels) : new Dictionary<string, string>(),
                containers
            );

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving pod: {ex.Message}" });
        }
    }

    [HttpGet("pods/{namespace}/{name}/logs")]
    public async Task<ActionResult<string>> GetPodLogs(string @namespace, string name, [FromQuery] string? container = null, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var logs = await _k8sService.GetPodLogsAsync(@namespace, name, container, kubeconfigPath, contextName).ConfigureAwait(false);

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving pod logs: {ex.Message}" });
        }
    }

    [HttpDelete("pods/{namespace}/{name}")]
    public async Task<ActionResult> DeletePod(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.DeletePodAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to delete pod" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting pod: {ex.Message}" });
        }
    }

    [HttpGet("pods/{namespace}/{name}/yaml")]
    public async Task<ActionResult<string>> GetPodYaml(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var yaml = await _k8sService.GetPodYamlAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
            return Ok(new { Yaml = yaml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error getting pod YAML: {ex.Message}" });
        }
    }

    #endregion

    #region Deployments

    [HttpGet("deployments")]
    public async Task<ActionResult<List<K8sDeploymentDto>>> GetDeployments([FromQuery] string @namespace, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var deployments = await _k8sService.ListDeploymentsAsync(@namespace, kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = deployments.Select(dep => new K8sDeploymentDto(
                dep.Metadata?.Name ?? "",
                dep.Metadata?.NamespaceProperty ?? "",
                dep.Spec?.Replicas ?? 0,
                dep.Status?.ReadyReplicas ?? 0,
                dep.Status?.AvailableReplicas ?? 0,
                dep.Metadata?.CreationTimestamp ?? DateTime.MinValue,
                dep.Metadata?.Labels != null ? new Dictionary<string, string>(dep.Metadata.Labels) : new Dictionary<string, string>()
            )).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving deployments: {ex.Message}" });
        }
    }

    [HttpPost("deployments/{namespace}/{name}/scale")]
    public async Task<ActionResult> ScaleDeployment(string @namespace, string name, [FromQuery] int replicas, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.ScaleDeploymentAsync(@namespace, name, replicas, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to scale deployment" });

            return Ok(new { Message = $"Deployment scaled to {replicas} replicas" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error scaling deployment: {ex.Message}" });
        }
    }

    [HttpGet("deployments/{namespace}/{name}/yaml")]
    public async Task<ActionResult<string>> GetDeploymentYaml(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var yaml = await _k8sService.GetDeploymentYamlAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
            return Ok(new { Yaml = yaml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error getting deployment YAML: {ex.Message}" });
        }
    }

    [HttpDelete("deployments/{namespace}/{name}")]
    public async Task<ActionResult> DeleteDeployment(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.DeleteDeploymentAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to delete deployment" });

            return Ok(new { Message = $"Deployment '{name}' deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting deployment: {ex.Message}" });
        }
    }

    #endregion

    #region Services

    [HttpGet("services")]
    public async Task<ActionResult<List<K8sServiceDto>>> GetServices([FromQuery] string @namespace, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var services = await _k8sService.ListServicesAsync(@namespace, kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = services.Select(svc =>
            {
                var ports = svc.Spec?.Ports?.Select(p => $"{p.Port}/{p.Protocol}").ToList() ?? new List<string>();
                var externalIPs = svc.Status?.LoadBalancer?.Ingress?.Select(i => i.Ip ?? i.Hostname ?? "").Where(ip => !string.IsNullOrEmpty(ip)).ToList() ?? new List<string>();

                return new K8sServiceDto(
                    svc.Metadata?.Name ?? "",
                    svc.Metadata?.NamespaceProperty ?? "",
                    svc.Spec?.Type ?? "",
                    svc.Spec?.ClusterIP ?? "",
                    externalIPs,
                    ports,
                    svc.Metadata?.CreationTimestamp ?? DateTime.MinValue,
                    svc.Metadata?.Labels != null ? new Dictionary<string, string>(svc.Metadata.Labels) : new Dictionary<string, string>()
                );
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving services: {ex.Message}" });
        }
    }

    [HttpGet("services/{namespace}/{name}/yaml")]
    public async Task<ActionResult<string>> GetServiceYaml(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var yaml = await _k8sService.GetServiceYamlAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
            return Ok(new { Yaml = yaml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error getting service YAML: {ex.Message}" });
        }
    }

    [HttpDelete("services/{namespace}/{name}")]
    public async Task<ActionResult> DeleteService(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.DeleteServiceAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to delete service" });

            return Ok(new { Message = $"Service '{name}' deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting service: {ex.Message}" });
        }
    }

    #endregion

    #region ConfigMaps

    [HttpGet("configmaps")]
    public async Task<ActionResult<List<K8sConfigMapDto>>> GetConfigMaps([FromQuery] string @namespace, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var configMaps = await _k8sService.ListConfigMapsAsync(@namespace, kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = configMaps.Select(cm => new K8sConfigMapDto(
                cm.Metadata?.Name ?? "",
                cm.Metadata?.NamespaceProperty ?? "",
                cm.Data?.Keys.Count ?? 0,
                cm.Metadata?.CreationTimestamp ?? DateTime.MinValue
            )).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving config maps: {ex.Message}" });
        }
    }

    [HttpGet("configmaps/{namespace}/{name}/yaml")]
    public async Task<ActionResult<string>> GetConfigMapYaml(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var yaml = await _k8sService.GetConfigMapYamlAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
            return Ok(new { Yaml = yaml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error getting configmap YAML: {ex.Message}" });
        }
    }

    [HttpDelete("configmaps/{namespace}/{name}")]
    public async Task<ActionResult> DeleteConfigMap(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.DeleteConfigMapAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to delete configmap" });

            return Ok(new { Message = $"ConfigMap '{name}' deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting configmap: {ex.Message}" });
        }
    }

    #endregion

    #region Secrets

    [HttpGet("secrets")]
    public async Task<ActionResult<List<K8sSecretDto>>> GetSecrets([FromQuery] string @namespace, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var secrets = await _k8sService.ListSecretsAsync(@namespace, kubeconfigPath, contextName).ConfigureAwait(false);

            var dtos = secrets.Select(secret => new K8sSecretDto(
                secret.Metadata?.Name ?? "",
                secret.Metadata?.NamespaceProperty ?? "",
                secret.Type ?? "",
                secret.Data?.Keys.Count ?? 0,
                secret.Metadata?.CreationTimestamp ?? DateTime.MinValue
            )).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving secrets: {ex.Message}" });
        }
    }

    [HttpGet("secrets/{namespace}/{name}/yaml")]
    public async Task<ActionResult<string>> GetSecretYaml(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var yaml = await _k8sService.GetSecretYamlAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);
            return Ok(new { Yaml = yaml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error getting secret YAML: {ex.Message}" });
        }
    }

    [HttpDelete("secrets/{namespace}/{name}")]
    public async Task<ActionResult> DeleteSecret(string @namespace, string name, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var success = await _k8sService.DeleteSecretAsync(@namespace, name, kubeconfigPath, contextName).ConfigureAwait(false);

            if (!success)
                return StatusCode(500, new { Message = "Failed to delete secret" });

            return Ok(new { Message = $"Secret '{name}' deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error deleting secret: {ex.Message}" });
        }
    }

    #endregion

    #region YAML Apply

    [HttpPost("apply")]
    public async Task<ActionResult<ApplyYamlResponse>> ApplyYaml([FromBody] ApplyYamlRequest request, [FromQuery] int? clusterId = null)
    {
        try
        {
            var (kubeconfigPath, contextName) = await GetClusterConfigAsync(clusterId).ConfigureAwait(false);
            var (success, message) = await _k8sService.ApplyYamlAsync(request.YamlContent, kubeconfigPath, contextName).ConfigureAwait(false);

            return Ok(new ApplyYamlResponse(success, message, null));
        }
        catch (Exception ex)
        {
            return Ok(new ApplyYamlResponse(false, $"Error applying YAML: {ex.Message}", null));
        }
    }

    #endregion
}
