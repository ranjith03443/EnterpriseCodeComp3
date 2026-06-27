using System.Text;
using System.Text.Json;
using ECIP.Core.Interfaces;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

/// <summary>
/// Repository Intelligence Dashboard API endpoints.
/// </summary>
[ApiController]
[Route("api/repository")]
public class RepositoryDashboardController : ControllerBase
{
    private readonly IMetadataExtractionService _metadataService;
    private readonly IRepositoryService _repositoryService;
    private readonly ILogger<RepositoryDashboardController> _logger;

    public RepositoryDashboardController(
        IMetadataExtractionService metadataService,
        IRepositoryService repositoryService,
        ILogger<RepositoryDashboardController> logger)
    {
        _metadataService = metadataService;
        _repositoryService = repositoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the full repository intelligence dashboard data.
    /// </summary>
    [HttpGet("{id:guid}/dashboard")]
    public async Task<IActionResult> GetDashboard(Guid id)
    {
        try
        {
            var repository = await _repositoryService.GetRepositoryAsync(id);
            if (repository is null)
            {
                return NotFound(new ApiResponse<string>("Repository not found.", false));
            }

            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);
            var languages = await _metadataService.GetLanguageSummaryAsync(id);
            var projects = await _metadataService.GetProjectsAsync(id);
            var folderStats = await _metadataService.GetFolderStatisticsAsync(id);

            var totalFiles = statistics?.TotalFiles ?? 0;
            var totalSizeBytes = statistics?.TotalSizeBytes ?? 0;
            var avgFileSize = totalFiles > 0 ? totalSizeBytes / totalFiles : 0;

            var dashboard = new RepositoryDashboardDto
            {
                RepositoryId = id,
                RepositoryName = repository.Name,
                RepositoryType = repository.RepositoryType.ToString(),
                RepositoryRoot = repository.LocalPath,
                RepositorySize = FormatFileSize(totalSizeBytes),
                RepositorySizeBytes = totalSizeBytes,
                TotalFiles = totalFiles,
                TotalFolders = statistics?.TotalFolders ?? 0,
                SourceFiles = statistics?.SourceFiles ?? 0,
                ConfigurationFiles = statistics?.ConfigurationFiles ?? 0,
                DocumentationFiles = statistics?.DocumentationFiles ?? 0,
                UnknownFiles = totalFiles - (statistics?.SourceFiles ?? 0) - (statistics?.ConfigurationFiles ?? 0) - (statistics?.DocumentationFiles ?? 0),
                ProjectCount = statistics?.ProjectCount ?? 0,
                LanguageCount = statistics?.LanguageCount ?? 0,
                LargestFileName = statistics?.LargestFileName ?? string.Empty,
                LargestFileSize = FormatFileSize(statistics?.LargestFileSize ?? 0),
                AverageFileSize = FormatFileSize(avgFileSize),
                LastScanDate = repository.LastScanDate,
                ScanDuration = statistics?.ScanDuration ?? string.Empty,
                Status = repository.Status,
                Languages = languages.Select(l => new LanguageSummaryDto
                {
                    Language = l.Language,
                    FileCount = l.FileCount,
                    TotalSize = l.TotalSize,
                    TotalSizeFormatted = FormatFileSize(l.TotalSize),
                    Percentage = l.Percentage,
                    Extension = l.Extension
                }).ToList(),
                Projects = projects.Select(p => new RepositoryProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProjectType = p.ProjectType,
                    FileName = p.FileName,
                    RelativePath = p.RelativePath,
                    Directory = p.Directory
                }).ToList(),
                Folders = folderStats.Select(fs => new FolderStatisticsDto
                {
                    FolderName = fs.FolderName,
                    ParentFolder = fs.ParentFolder,
                    RelativePath = fs.RelativePath,
                    NumberOfFiles = fs.NumberOfFiles,
                    TotalSize = fs.TotalSize,
                    TotalSizeFormatted = FormatFileSize(fs.TotalSize),
                    Depth = fs.Depth
                }).ToList(),
                Timeline = new List<RepositoryTimelineDto>
                {
                    new()
                    {
                        ScanDate = repository.LastScanDate ?? DateTime.UtcNow,
                        Duration = statistics?.ScanDuration ?? "N/A",
                        FilesProcessed = totalFiles,
                        FoldersProcessed = statistics?.TotalFolders ?? 0,
                        ProjectsFound = statistics?.ProjectCount ?? 0,
                        LanguagesDetected = statistics?.LanguageCount ?? 0,
                        Status = "Completed"
                    }
                }
            };

            _logger.LogInformation("Dashboard loaded for repository {RepositoryName}", repository.Name);
            return Ok(new ApiResponse<RepositoryDashboardDto>(dashboard, "Dashboard data retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Gets language breakdown for a repository.
    /// </summary>
    [HttpGet("{id:guid}/languages")]
    public async Task<IActionResult> GetLanguages(Guid id)
    {
        try
        {
            var languages = await _metadataService.GetLanguageSummaryAsync(id);
            var dto = languages.Select(l => new LanguageSummaryDto
            {
                Language = l.Language,
                FileCount = l.FileCount,
                TotalSize = l.TotalSize,
                TotalSizeFormatted = FormatFileSize(l.TotalSize),
                Percentage = l.Percentage,
                Extension = l.Extension
            }).ToList();

            return Ok(new ApiResponse<List<LanguageSummaryDto>>(dto, $"{dto.Count} languages found."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving languages for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Gets folder tree for a repository.
    /// </summary>
    [HttpGet("{id:guid}/folders")]
    public async Task<IActionResult> GetFolders(Guid id)
    {
        try
        {
            var folders = await _metadataService.GetFolderStatisticsAsync(id);
            var dto = folders.Select(fs => new FolderStatisticsDto
            {
                FolderName = fs.FolderName,
                ParentFolder = fs.ParentFolder,
                RelativePath = fs.RelativePath,
                NumberOfFiles = fs.NumberOfFiles,
                TotalSize = fs.TotalSize,
                TotalSizeFormatted = FormatFileSize(fs.TotalSize),
                Depth = fs.Depth
            }).ToList();

            return Ok(new ApiResponse<List<FolderStatisticsDto>>(dto, $"{dto.Count} folders found."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving folders for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Gets scan timeline for a repository.
    /// </summary>
    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        try
        {
            var repository = await _repositoryService.GetRepositoryAsync(id);
            if (repository is null)
            {
                return NotFound(new ApiResponse<string>("Repository not found.", false));
            }

            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);

            var timeline = new List<RepositoryTimelineDto>();
            if (statistics != null)
            {
                timeline.Add(new RepositoryTimelineDto
                {
                    ScanDate = repository.LastScanDate ?? statistics.GeneratedDate,
                    Duration = statistics.ScanDuration,
                    FilesProcessed = statistics.TotalFiles,
                    FoldersProcessed = statistics.TotalFolders,
                    ProjectsFound = statistics.ProjectCount,
                    LanguagesDetected = statistics.LanguageCount,
                    Status = "Completed"
                });
            }

            return Ok(new ApiResponse<List<RepositoryTimelineDto>>(timeline, "Timeline retrieved."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timeline for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Exports repository summary as JSON.
    /// </summary>
    [HttpGet("{id:guid}/export/json")]
    public async Task<IActionResult> ExportJson(Guid id)
    {
        try
        {
            var repository = await _repositoryService.GetRepositoryAsync(id);
            if (repository is null)
            {
                return NotFound(new ApiResponse<string>("Repository not found.", false));
            }

            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);
            var languages = await _metadataService.GetLanguageSummaryAsync(id);
            var projects = await _metadataService.GetProjectsAsync(id);

            var export = new
            {
                repository.Name,
                RepositoryType = repository.RepositoryType.ToString(),
                repository.LocalPath,
                repository.Branch,
                repository.Status,
                repository.LastScanDate,
                Statistics = statistics != null ? new
                {
                    statistics.TotalFiles,
                    statistics.TotalFolders,
                    statistics.SourceFiles,
                    statistics.ConfigurationFiles,
                    statistics.DocumentationFiles,
                    TotalSize = FormatFileSize(statistics.TotalSizeBytes),
                    statistics.LargestFileName,
                    LargestFileSize = FormatFileSize(statistics.LargestFileSize)
                } : null,
                Languages = languages.Select(l => new { l.Language, l.FileCount, l.Percentage, Size = FormatFileSize(l.TotalSize) }),
                Projects = projects.Select(p => new { p.Name, p.ProjectType, p.FileName, p.RelativePath })
            };

            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);

            _logger.LogInformation("Export completed (JSON) for repository {RepositoryName}", repository.Name);
            return File(bytes, "application/json", $"{repository.Name}_summary.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting JSON for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Exports repository summary as CSV.
    /// </summary>
    [HttpGet("{id:guid}/export/csv")]
    public async Task<IActionResult> ExportCsv(Guid id)
    {
        try
        {
            var repository = await _repositoryService.GetRepositoryAsync(id);
            if (repository is null)
            {
                return NotFound(new ApiResponse<string>("Repository not found.", false));
            }

            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);
            var languages = await _metadataService.GetLanguageSummaryAsync(id);
            var projects = await _metadataService.GetProjectsAsync(id);

            var sb = new StringBuilder();
            sb.AppendLine("Section,Key,Value");
            sb.AppendLine($"Repository,Name,{repository.Name}");
            sb.AppendLine($"Repository,Type,{repository.RepositoryType}");
            sb.AppendLine($"Repository,Branch,{repository.Branch}");
            sb.AppendLine($"Repository,Status,{repository.Status}");
            sb.AppendLine($"Repository,Path,\"{repository.LocalPath}\"");
            sb.AppendLine($"Repository,LastScan,{repository.LastScanDate}");

            if (statistics != null)
            {
                sb.AppendLine($"Statistics,TotalFiles,{statistics.TotalFiles}");
                sb.AppendLine($"Statistics,TotalFolders,{statistics.TotalFolders}");
                sb.AppendLine($"Statistics,SourceFiles,{statistics.SourceFiles}");
                sb.AppendLine($"Statistics,ConfigFiles,{statistics.ConfigurationFiles}");
                sb.AppendLine($"Statistics,DocFiles,{statistics.DocumentationFiles}");
                sb.AppendLine($"Statistics,TotalSize,{FormatFileSize(statistics.TotalSizeBytes)}");
                sb.AppendLine($"Statistics,LargestFile,{statistics.LargestFileName}");
            }

            sb.AppendLine();
            sb.AppendLine("Language,Files,Percentage,Size");
            foreach (var lang in languages)
            {
                sb.AppendLine($"{lang.Language},{lang.FileCount},{lang.Percentage}%,{FormatFileSize(lang.TotalSize)}");
            }

            sb.AppendLine();
            sb.AppendLine("Project,Type,File,Path");
            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name},{project.ProjectType},{project.FileName},\"{project.RelativePath}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            _logger.LogInformation("Export completed (CSV) for repository {RepositoryName}", repository.Name);
            return File(bytes, "text/csv", $"{repository.Name}_summary.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting CSV for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
