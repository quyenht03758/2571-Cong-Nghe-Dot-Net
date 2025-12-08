using Blazored.LocalStorage;
using BlazorWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorWeb.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly CustomAuthStateProvider? _authStateProvider;
    private const string API_BASE_URL = "http://localhost:5000";
    private const string TOKEN_KEY = "authToken";

    public ApiClient(HttpClient httpClient, ILocalStorageService localStorage, CustomAuthStateProvider? authStateProvider = null)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _httpClient.BaseAddress = new Uri(API_BASE_URL);
    }

    public async Task InitializeAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync(TOKEN_KEY);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // ==================== AUTH ====================

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new LoginRequest(username, password));

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Token != null)
            {
                await _localStorage.SetItemAsStringAsync(TOKEN_KEY, result.Token);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Token);
                
                // Notify authentication state changed
                _authStateProvider?.NotifyUserAuthentication(result.Token);
            }
            return result;
        }
        return null;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TOKEN_KEY);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        // Notify authentication state changed
        _authStateProvider?.NotifyUserLogout();
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            // Ensure token is set from localStorage
            await InitializeAsync();
            return await _httpClient.GetFromJsonAsync<UserDto>("/api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    // ==================== TEMPLATES ====================

    public async Task<List<TemplateDto>> GetTemplatesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TemplateDto>>("/api/templates") ?? new();
    }

    public async Task<TemplateDto?> GetTemplateByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TemplateDto>($"/api/templates/{id}");
    }

    public async Task<TemplateDto?> CreateTemplateAsync(CreateTemplateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/templates", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateDto>();
    }

    public async Task UpdateTemplateAsync(int id, UpdateTemplateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/templates/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTemplateAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/templates/{id}");
        response.EnsureSuccessStatusCode();
    }

    // ==================== FAVORITES ====================

    public async Task<List<FavoriteDto>> GetFavoritesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<FavoriteDto>>("/api/favorites") ?? new();
    }

    public async Task<FavoriteDto?> CreateFavoriteAsync(CreateFavoriteRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/favorites", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FavoriteDto>();
    }

    public async Task DeleteFavoriteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/favorites/{id}");
        response.EnsureSuccessStatusCode();
    }

    // ==================== AUDIT LOGS ====================

    public async Task<List<AuditLogDto>> GetAuditLogsAsync(int page = 1, int pageSize = 50)
    {
        return await _httpClient.GetFromJsonAsync<List<AuditLogDto>>($"/api/auditlogs?page={page}&pageSize={pageSize}") ?? new();
    }

    // ==================== SESSIONS ====================

    public async Task<List<SessionDto>> GetMySessionsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SessionDto>>("/api/sessions/my") ?? new();
    }

    public async Task<List<SessionDto>> GetActiveSessionsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SessionDto>>("/api/sessions/active") ?? new();
    }

    public async Task DeleteSessionAsync(long sessionId)
    {
        var response = await _httpClient.DeleteAsync($"/api/sessions/{sessionId}");
        response.EnsureSuccessStatusCode();
    }

    // ==================== CLUSTERS ====================

    public async Task<List<ClusterDto>> GetClustersAsync()
    {
        await InitializeAsync();
        return await _httpClient.GetFromJsonAsync<List<ClusterDto>>("/api/clusters") ?? new();
    }

    public async Task<ClusterDto?> GetClusterByIdAsync(int id)
    {
        await InitializeAsync();
        return await _httpClient.GetFromJsonAsync<ClusterDto>($"/api/clusters/{id}");
    }

    public async Task<ClusterDto?> CreateClusterAsync(CreateClusterRequest request)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/clusters", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClusterDto>();
    }

    public async Task DeleteClusterAsync(int id)
    {
        await InitializeAsync();
        var response = await _httpClient.DeleteAsync($"/api/clusters/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task SetDefaultClusterAsync(int id)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsync($"/api/clusters/{id}/set-default", null);
        response.EnsureSuccessStatusCode();
    }

    // ==================== KUBERNETES RESOURCES ====================

    public async Task<List<string>> GetContextsAsync(string? kubeconfigPath = null)
    {
        await InitializeAsync();
        var url = "/api/k8s/contexts";
        if (!string.IsNullOrEmpty(kubeconfigPath))
        {
            url += $"?kubeconfigPath={Uri.EscapeDataString(kubeconfigPath)}";
        }
        return await _httpClient.GetFromJsonAsync<List<string>>(url) ?? new();
    }

    public async Task<List<K8sNamespaceDto>> GetNamespacesAsync(int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue ? $"/api/k8s/namespaces?clusterId={clusterId}" : "/api/k8s/namespaces";
        return await _httpClient.GetFromJsonAsync<List<K8sNamespaceDto>>(url) ?? new();
    }

    public async Task CreateNamespaceAsync(string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue ? $"/api/k8s/namespaces?clusterId={clusterId}" : "/api/k8s/namespaces";
        var response = await _httpClient.PostAsJsonAsync(url, new CreateNamespaceRequest(name));
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<K8sPodDto>> GetPodsAsync(string @namespace, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/pods?namespace={@namespace}&clusterId={clusterId}" 
            : $"/api/k8s/pods?namespace={@namespace}";
        return await _httpClient.GetFromJsonAsync<List<K8sPodDto>>(url) ?? new();
    }

    public async Task<K8sPodDto?> GetPodAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/pods/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/pods/{@namespace}/{name}";
        return await _httpClient.GetFromJsonAsync<K8sPodDto>(url);
    }

    public async Task<string> GetPodLogsAsync(string @namespace, string name, string? container = null, int? clusterId = null)
    {
        await InitializeAsync();
        var url = $"/api/k8s/pods/{@namespace}/{name}/logs";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(container)) queryParams.Add($"container={container}");
        if (clusterId.HasValue) queryParams.Add($"clusterId={clusterId}");
        if (queryParams.Any()) url += "?" + string.Join("&", queryParams);
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeletePodAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/pods/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/pods/{@namespace}/{name}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<K8sDeploymentDto>> GetDeploymentsAsync(string @namespace, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/deployments?namespace={@namespace}&clusterId={clusterId}" 
            : $"/api/k8s/deployments?namespace={@namespace}";
        return await _httpClient.GetFromJsonAsync<List<K8sDeploymentDto>>(url) ?? new();
    }

    public async Task ScaleDeploymentAsync(string @namespace, string name, int replicas, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/deployments/{@namespace}/{name}/scale?replicas={replicas}&clusterId={clusterId}" 
            : $"/api/k8s/deployments/{@namespace}/{name}/scale?replicas={replicas}";
        var response = await _httpClient.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDeploymentAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/deployments/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/deployments/{@namespace}/{name}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<K8sServiceDto>> GetServicesAsync(string @namespace, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/services?namespace={@namespace}&clusterId={clusterId}" 
            : $"/api/k8s/services?namespace={@namespace}";
        return await _httpClient.GetFromJsonAsync<List<K8sServiceDto>>(url) ?? new();
    }

    public async Task DeleteServiceAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/services/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/services/{@namespace}/{name}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<K8sConfigMapDto>> GetConfigMapsAsync(string @namespace, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/configmaps?namespace={@namespace}&clusterId={clusterId}" 
            : $"/api/k8s/configmaps?namespace={@namespace}";
        return await _httpClient.GetFromJsonAsync<List<K8sConfigMapDto>>(url) ?? new();
    }

    public async Task DeleteConfigMapAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/configmaps/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/configmaps/{@namespace}/{name}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<K8sSecretDto>> GetSecretsAsync(string @namespace, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/secrets?namespace={@namespace}&clusterId={clusterId}" 
            : $"/api/k8s/secrets?namespace={@namespace}";
        return await _httpClient.GetFromJsonAsync<List<K8sSecretDto>>(url) ?? new();
    }

    public async Task DeleteSecretAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/secrets/{@namespace}/{name}?clusterId={clusterId}" 
            : $"/api/k8s/secrets/{@namespace}/{name}";
        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ApplyYamlResponse> ApplyYamlAsync(ApplyYamlRequest request, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue ? $"/api/k8s/apply?clusterId={clusterId}" : "/api/k8s/apply";
        var response = await _httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApplyYamlResponse>() 
            ?? new ApplyYamlResponse(false, "Unknown error", null);
    }

    public async Task<ApplyYamlResponse> ApplyYamlAsync(string yamlContent, int? clusterId = null)
    {
        return await ApplyYamlAsync(new ApplyYamlRequest(yamlContent), clusterId);
    }

    // YAML Export methods
    public async Task<string> GetPodYamlAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/pods/{@namespace}/{name}/yaml?clusterId={clusterId}" 
            : $"/api/k8s/pods/{@namespace}/{name}/yaml";
        var response = await _httpClient.GetFromJsonAsync<YamlResponse>(url);
        return response?.Yaml ?? "";
    }

    public async Task<string> GetDeploymentYamlAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/deployments/{@namespace}/{name}/yaml?clusterId={clusterId}" 
            : $"/api/k8s/deployments/{@namespace}/{name}/yaml";
        var response = await _httpClient.GetFromJsonAsync<YamlResponse>(url);
        return response?.Yaml ?? "";
    }

    public async Task<string> GetServiceYamlAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/services/{@namespace}/{name}/yaml?clusterId={clusterId}" 
            : $"/api/k8s/services/{@namespace}/{name}/yaml";
        var response = await _httpClient.GetFromJsonAsync<YamlResponse>(url);
        return response?.Yaml ?? "";
    }

    public async Task<string> GetConfigMapYamlAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/configmaps/{@namespace}/{name}/yaml?clusterId={clusterId}" 
            : $"/api/k8s/configmaps/{@namespace}/{name}/yaml";
        var response = await _httpClient.GetFromJsonAsync<YamlResponse>(url);
        return response?.Yaml ?? "";
    }

    public async Task<string> GetSecretYamlAsync(string @namespace, string name, int? clusterId = null)
    {
        await InitializeAsync();
        var url = clusterId.HasValue 
            ? $"/api/k8s/secrets/{@namespace}/{name}/yaml?clusterId={clusterId}" 
            : $"/api/k8s/secrets/{@namespace}/{name}/yaml";
        var response = await _httpClient.GetFromJsonAsync<YamlResponse>(url);
        return response?.Yaml ?? "";
    }

    // ==================== USER MANAGEMENT ====================

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        await InitializeAsync();
        return await _httpClient.GetFromJsonAsync<List<UserResponse>>("/api/users") ?? new();
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        await InitializeAsync();
        return await _httpClient.GetFromJsonAsync<UserResponse>($"/api/users/{id}");
    }

    public async Task<int> CreateUserAsync(CreateUserRequestDto request)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/users", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return result?["id"] ?? 0;
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequestDto request)
    {
        await InitializeAsync();
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResetUserPasswordAsync(int id, string newPassword)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsJsonAsync($"/api/users/{id}/reset-password", new ResetPasswordRequestDto(newPassword));
        response.EnsureSuccessStatusCode();
    }

    public async Task LockUserAsync(int id)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsync($"/api/users/{id}/lock", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnlockUserAsync(int id)
    {
        await InitializeAsync();
        var response = await _httpClient.PostAsync($"/api/users/{id}/unlock", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(int id)
    {
        await InitializeAsync();
        var response = await _httpClient.DeleteAsync($"/api/users/{id}");
        response.EnsureSuccessStatusCode();
    }
}
