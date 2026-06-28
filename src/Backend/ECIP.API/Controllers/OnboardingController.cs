using System.Text;
using System.Text.Json;
using ECIP.Core.Interfaces;
using ECIP.Infrastructure.Persistence;
using ECIP.Shared.DTOs.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly IRepositoryService _repositories;
    private readonly EcipDbContext _db;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<OnboardingController> _logger;

    public OnboardingController(
        IRepositoryService repositories,
        EcipDbContext db,
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<OnboardingController> logger)
    {
        _repositories = repositories;
        _db = db;
        _httpFactory = httpFactory;
        _config = config;
        _logger = logger;
    }

    [HttpPost("{repositoryId}/ask")]
    public async Task<IActionResult> Ask(Guid repositoryId, [FromBody] OnboardingQueryRequest request)
    {
        var repo = await _repositories.GetRepositoryAsync(repositoryId);
        if (repo is null) return NotFound();

        var files     = await _db.RepositoryFiles.Where(f => f.RepositoryId == repositoryId).ToListAsync();
        var projects  = await _db.RepositoryProjects.Where(p => p.RepositoryId == repositoryId).ToListAsync();
        var languages = await _db.RepositoryLanguages.Where(l => l.RepositoryId == repositoryId).ToListAsync();

        // Build a context summary
        var sb = new StringBuilder();
        sb.AppendLine($"Repository: {repo.Name}");
        sb.AppendLine($"Total files: {files.Count}");
        sb.AppendLine($"Languages: {string.Join(", ", languages.Select(l => $"{l.Name} ({l.FileCount} files)"))}");
        sb.AppendLine($"Projects: {string.Join(", ", projects.Select(p => p.Name))}");

        var byLang = files.GroupBy(f => f.Language)
                          .OrderByDescending(g => g.Count())
                          .Take(5);
        foreach (var g in byLang)
            sb.AppendLine($"  {g.Key} files: {string.Join(", ", g.Take(5).Select(f => f.FileName))}");

        // Keyword-scored retrieval: rank files by overlap with query tokens
        var queryTokens = request.Query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var chunks = files
            .Select(f => new
            {
                File  = f,
                Score = queryTokens.Count(t => f.FileName.ToLower().Contains(t) || f.RelativePath.ToLower().Contains(t)),
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(8)
            .Select(x => new SourceChunkDto
            {
                Id      = x.File.Id.ToString(),
                Source  = x.File.RelativePath,
                Content = $"{x.File.FileName} ({x.File.Language}, {x.File.FileSize} bytes)",
                Type    = "file",
                Score   = Math.Min(x.Score / 3.0, 1.0),
            })
            .ToList();

        // Also include project-level chunks
        var projectChunks = projects
            .Where(p => queryTokens.Any(t => p.Name.ToLower().Contains(t) || p.ProjectType.ToLower().Contains(t)))
            .Take(3)
            .Select(p => new SourceChunkDto
            {
                Id      = p.Id.ToString(),
                Source  = p.RelativePath,
                Content = $"{p.Name} ({p.ProjectType})",
                Type    = "project",
                Score   = 0.9,
            });
        chunks.AddRange(projectChunks);

        var aiRequest = new OnboardingRequestDto
        {
            RepositoryId = repositoryId,
            Query        = request.Query,
            Context      = sb.ToString(),
            Chunks       = chunks,
        };

        try
        {
            var aiBase = _config["AiService:BaseUrl"] ?? "http://localhost:8000";
            var client = _httpFactory.CreateClient();
            var json   = new StringContent(JsonSerializer.Serialize(aiRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }),
                                           Encoding.UTF8, "application/json");
            var resp   = await client.PostAsync($"{aiBase}/api/onboarding/ask", json);
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, "AI service error");

            var body    = await resp.Content.ReadAsStringAsync();
            var result  = JsonSerializer.Deserialize<OnboardingResponseDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onboarding ask failed for repository {Id}", repositoryId);
            return StatusCode(503, "AI service unavailable");
        }
    }
}

public class OnboardingQueryRequest
{
    public string Query { get; set; } = string.Empty;
}
