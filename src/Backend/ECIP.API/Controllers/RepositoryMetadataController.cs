using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

/// <summary>
/// API endpoints for repository metadata, statistics, and project discovery.
/// </summary>
[ApiController]
[Route("api/repository")]
public class RepositoryMetadataController : ControllerBase
{
    private readonly IMetadataExtractionService _metadataService;
    private readonly ILogger<RepositoryMetadataController> _logger;

    public RepositoryMetadataController(
        IMetadataExtractionService metadataService,
        ILogger<RepositoryMetadataController> logger)
    {
        _metadataService = metadataService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the repository summary including metadata.
    /// </summary>
    [HttpGet("{id:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        try
        {
            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);
            if (statistics is null)
            {
                return NotFound(new ApiResponse<string>("No metadata found. Please scan the repository first.", false));
            }

            var languages = await _metadataService.GetLanguageSummaryAsync(id);

            var summary = new RepositoryMetadataSummaryDto
            {
                RepositoryId = statistics.RepositoryId,
                RepositoryName = statistics.RepositoryName,
                RepositoryType = statistics.RepositoryType,
                RepositoryRoot = statistics.RepositoryRoot,
                TotalFiles = statistics.TotalFiles,
                TotalFolders = statistics.TotalFolders,
                SourceFiles = statistics.SourceFiles,
                ConfigurationFiles = statistics.ConfigurationFiles,
                DocumentationFiles = statistics.DocumentationFiles,
                ProjectCount = statistics.ProjectCount,
                LanguageCount = statistics.LanguageCount,
                TotalSizeBytes = statistics.TotalSizeBytes,
                TotalSizeFormatted = FormatFileSize(statistics.TotalSizeBytes),
                LargestFileName = statistics.LargestFileName,
                LargestFileSize = statistics.LargestFileSize,
                LargestFileSizeFormatted = FormatFileSize(statistics.LargestFileSize),
                LastScanDate = statistics.LastScanDate,
                ScanDuration = statistics.ScanDuration,
                Languages = languages.Select(l => l.Language).ToList()
            };

            return Ok(new ApiResponse<RepositoryMetadataSummaryDto>(summary, "Repository summary retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving summary for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Gets the repository statistics including language and folder breakdowns.
    /// </summary>
    [HttpGet("{id:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid id)
    {
        try
        {
            var statistics = await _metadataService.GetRepositoryStatisticsAsync(id);
            if (statistics is null)
            {
                return NotFound(new ApiResponse<string>("No statistics found. Please scan the repository first.", false));
            }

            var languages = await _metadataService.GetLanguageSummaryAsync(id);
            var projects = await _metadataService.GetProjectsAsync(id);
            var folderStats = await _metadataService.GetFolderStatisticsAsync(id);

            var dto = new RepositoryStatisticsDto
            {
                RepositoryId = statistics.RepositoryId,
                RepositoryName = statistics.RepositoryName,
                TotalFiles = statistics.TotalFiles,
                TotalFolders = statistics.TotalFolders,
                ProjectCount = statistics.ProjectCount,
                LanguageCount = statistics.LanguageCount,
                TotalSizeBytes = statistics.TotalSizeBytes,
                TotalSizeFormatted = FormatFileSize(statistics.TotalSizeBytes),
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
                FolderStatistics = folderStats.Select(fs => new FolderStatisticsDto
                {
                    FolderName = fs.FolderName,
                    ParentFolder = fs.ParentFolder,
                    RelativePath = fs.RelativePath,
                    NumberOfFiles = fs.NumberOfFiles,
                    TotalSize = fs.TotalSize,
                    TotalSizeFormatted = FormatFileSize(fs.TotalSize),
                    Depth = fs.Depth
                }).ToList()
            };

            return Ok(new ApiResponse<RepositoryStatisticsDto>(dto, "Repository statistics retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Gets the discovered projects within a repository.
    /// </summary>
    [HttpGet("{id:guid}/projects")]
    public async Task<IActionResult> GetProjects(Guid id)
    {
        try
        {
            var projects = await _metadataService.GetProjectsAsync(id);

            var dto = projects.Select(p => new RepositoryProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ProjectType = p.ProjectType,
                FileName = p.FileName,
                RelativePath = p.RelativePath,
                Directory = p.Directory
            }).ToList();

            return Ok(new ApiResponse<List<RepositoryProjectDto>>(dto, $"{dto.Count} projects discovered."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects for repository {RepositoryId}", id);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }

    /// <summary>
    /// Triggers metadata extraction for a repository.
    /// </summary>
    [HttpPost("{id:guid}/extract-metadata")]
    public async Task<IActionResult> ExtractMetadata(Guid id)
    {
        try
        {
            var result = await _metadataService.GenerateMetadataAsync(id);
            return Ok(new ApiResponse<RepositoryMetadataSummaryDto>(new RepositoryMetadataSummaryDto
            {
                RepositoryId = result.RepositoryId,
                RepositoryName = result.RepositoryName,
                RepositoryType = result.RepositoryType,
                RepositoryRoot = result.RepositoryRoot,
                TotalFiles = result.TotalFiles,
                TotalFolders = result.TotalFolders,
                SourceFiles = result.SourceFiles,
                ConfigurationFiles = result.ConfigurationFiles,
                DocumentationFiles = result.DocumentationFiles,
                ProjectCount = result.ProjectCount,
                LanguageCount = result.LanguageCount,
                TotalSizeBytes = result.TotalSizeBytes,
                TotalSizeFormatted = FormatFileSize(result.TotalSizeBytes),
                LargestFileName = result.LargestFileName,
                LargestFileSize = result.LargestFileSize,
                LargestFileSizeFormatted = FormatFileSize(result.LargestFileSize),
                LastScanDate = result.LastScanDate,
                ScanDuration = result.ScanDuration
            }, "Metadata extraction completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting metadata for repository {RepositoryId}", id);
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
