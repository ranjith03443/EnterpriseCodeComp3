using System.Text.Json;
using ECIP.Shared.DTOs;

namespace ECIP.Web.Services;

/// <summary>
/// Service for communicating with the ECIP.API backend.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Gets the health status of the API.
    /// </summary>
    Task<HealthResponse?> GetHealthAsync();

    /// <summary>
    /// Gets the version information of the API.
    /// </summary>
    Task<VersionResponse?> GetVersionAsync();

    /// <summary>
    /// Gets the system status of all services.
    /// </summary>
    Task<SystemStatusResponse?> GetSystemStatusAsync();

    /// <summary>
    /// Gets the repository metadata summary.
    /// </summary>
    Task<RepositoryMetadataSummaryDto?> GetRepositorySummaryAsync(Guid repositoryId);

    /// <summary>
    /// Gets the repository statistics.
    /// </summary>
    Task<RepositoryStatisticsDto?> GetRepositoryStatisticsAsync(Guid repositoryId);

    /// <summary>
    /// Gets the discovered projects for a repository.
    /// </summary>
    Task<List<RepositoryProjectDto>?> GetRepositoryProjectsAsync(Guid repositoryId);

    /// <summary>
    /// Triggers metadata extraction for a repository.
    /// </summary>
    Task<RepositoryMetadataSummaryDto?> ExtractMetadataAsync(Guid repositoryId);

    /// <summary>
    /// Triggers a repository scan.
    /// </summary>
    Task<bool> ScanRepositoryAsync(Guid repositoryId);

    /// <summary>
    /// Gets the full repository intelligence dashboard.
    /// </summary>
    Task<RepositoryDashboardDto?> GetRepositoryDashboardAsync(Guid repositoryId);

    /// <summary>
    /// Gets language breakdown for a repository.
    /// </summary>
    Task<List<LanguageSummaryDto>?> GetRepositoryLanguagesAsync(Guid repositoryId);

    /// <summary>
    /// Gets folder statistics for a repository.
    /// </summary>
    Task<List<FolderStatisticsDto>?> GetRepositoryFoldersAsync(Guid repositoryId);

    /// <summary>
    /// Gets scan timeline for a repository.
    /// </summary>
    Task<List<RepositoryTimelineDto>?> GetRepositoryTimelineAsync(Guid repositoryId);

    /// <summary>
    /// Gets export JSON URL for a repository.
    /// </summary>
    string GetExportJsonUrl(Guid repositoryId);

    /// <summary>
    /// Gets export CSV URL for a repository.
    /// </summary>
    string GetExportCsvUrl(Guid repositoryId);
}

/// <summary>
/// Implementation of the API service.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets the health status of the API.
    /// </summary>
    public async Task<HealthResponse?> GetHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/health");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<HealthResponse>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Health check failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling health endpoint");
            return null;
        }
    }

    /// <summary>
    /// Gets the version information of the API.
    /// </summary>
    public async Task<VersionResponse?> GetVersionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/version");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<VersionResponse>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Version endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling version endpoint");
            return null;
        }
    }

    /// <summary>
    /// Gets the system status of all services.
    /// </summary>
    public async Task<SystemStatusResponse?> GetSystemStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/system");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<SystemStatusResponse>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("System status endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling system status endpoint");
            return null;
        }
    }

    /// <summary>
    /// Gets the repository metadata summary.
    /// </summary>
    public async Task<RepositoryMetadataSummaryDto?> GetRepositorySummaryAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/summary");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<RepositoryMetadataSummaryDto>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Repository summary endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling repository summary endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets the repository statistics.
    /// </summary>
    public async Task<RepositoryStatisticsDto?> GetRepositoryStatisticsAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/statistics");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<RepositoryStatisticsDto>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Repository statistics endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling repository statistics endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets the discovered projects for a repository.
    /// </summary>
    public async Task<List<RepositoryProjectDto>?> GetRepositoryProjectsAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/projects");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<List<RepositoryProjectDto>>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Repository projects endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling repository projects endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Triggers metadata extraction for a repository.
    /// </summary>
    public async Task<RepositoryMetadataSummaryDto?> ExtractMetadataAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/repository/{repositoryId}/extract-metadata", null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<RepositoryMetadataSummaryDto>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Metadata extraction endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling metadata extraction endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Triggers a repository scan.
    /// </summary>
    public async Task<bool> ScanRepositoryAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/repositories/{repositoryId}/scan", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling scan endpoint for {RepositoryId}", repositoryId);
            return false;
        }
    }

    /// <summary>
    /// Gets the full repository intelligence dashboard.
    /// </summary>
    public async Task<RepositoryDashboardDto?> GetRepositoryDashboardAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/dashboard");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<RepositoryDashboardDto>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Dashboard endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling dashboard endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets language breakdown for a repository.
    /// </summary>
    public async Task<List<LanguageSummaryDto>?> GetRepositoryLanguagesAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/languages");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<List<LanguageSummaryDto>>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Languages endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling languages endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets folder statistics for a repository.
    /// </summary>
    public async Task<List<FolderStatisticsDto>?> GetRepositoryFoldersAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/folders");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<List<FolderStatisticsDto>>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Folders endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling folders endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets scan timeline for a repository.
    /// </summary>
    public async Task<List<RepositoryTimelineDto>?> GetRepositoryTimelineAsync(Guid repositoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/repository/{repositoryId}/timeline");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<ApiResponse<List<RepositoryTimelineDto>>>(json, JsonOptions);
                return content?.Data;
            }

            _logger.LogWarning("Timeline endpoint failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling timeline endpoint for {RepositoryId}", repositoryId);
            return null;
        }
    }

    /// <summary>
    /// Gets export JSON URL for a repository.
    /// </summary>
    public string GetExportJsonUrl(Guid repositoryId)
    {
        return $"{_httpClient.BaseAddress}api/repository/{repositoryId}/export/json";
    }

    /// <summary>
    /// Gets export CSV URL for a repository.
    /// </summary>
    public string GetExportCsvUrl(Guid repositoryId)
    {
        return $"{_httpClient.BaseAddress}api/repository/{repositoryId}/export/csv";
    }
}
