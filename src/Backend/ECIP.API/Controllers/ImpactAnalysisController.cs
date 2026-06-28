using System.Text;
using System.Text.Json;
using ECIP.Core.Interfaces;
using ECIP.Infrastructure.Persistence;
using ECIP.Shared.DTOs.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/impact")]
public class ImpactAnalysisController : ControllerBase
{
    private readonly IRepositoryService _repositories;
    private readonly EcipDbContext _db;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<ImpactAnalysisController> _logger;

    private static readonly List<(string Name, string Color, string Icon, string[] Keywords)> _layerRules =
    [
        ("Presentation",   "primary",  "bi-window",         ["web", "ui", "frontend", "mvc", "razor", "portal", "client"]),
        ("API Layer",      "success",  "bi-hdd-network",    ["api", "gateway", "rest", "endpoint", "grpc"]),
        ("Domain / Core",  "warning",  "bi-bullseye",       ["core", "domain", "business", "model", "entity"]),
        ("Infrastructure", "danger",   "bi-database",       ["infrastructure", "persistence", "data", "migration", "ef", "db"]),
        ("Shared / DTOs",  "info",     "bi-share",          ["shared", "dto", "contract", "common"]),
        ("AI Services",    "purple",   "bi-robot",          ["ai", "aiservice", "llm", "ml", "prompt", "embedding"]),
        ("Tests",          "secondary","bi-check2-square",  ["test", "spec", "fixture", "mock"]),
    ];

    public ImpactAnalysisController(
        IRepositoryService repositories,
        EcipDbContext db,
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<ImpactAnalysisController> logger)
    {
        _repositories = repositories;
        _db = db;
        _httpFactory = httpFactory;
        _config = config;
        _logger = logger;
    }

    [HttpGet("{repositoryId}")]
    public async Task<IActionResult> Analyze(Guid repositoryId, [FromQuery] string component, [FromQuery] string changeType = "modify")
    {
        if (string.IsNullOrWhiteSpace(component))
            return BadRequest("component query param is required");

        var repo = await _repositories.GetRepositoryAsync(repositoryId);
        if (repo is null) return NotFound();

        var allFiles    = await _db.RepositoryFiles.Where(f => f.RepositoryId == repositoryId).ToListAsync();
        var allProjects = await _db.RepositoryProjects.Where(p => p.RepositoryId == repositoryId).ToListAsync();

        var compLower = component.ToLower();
        var tokens    = compLower.Split(['.', '/', '\\', ' '], StringSplitOptions.RemoveEmptyEntries);

        // Direct matches: files whose name or path contains the component tokens
        var directFiles = allFiles
            .Where(f => tokens.Any(t =>
                f.FileName.ToLower().Contains(t) ||
                f.RelativePath.ToLower().Contains(t)))
            .ToList();

        // Layer classify each affected file
        var affectedFiles = directFiles.Select(f =>
        {
            var layer = ClassifyLayer(f.RelativePath, f.FileName);
            return new AffectedFileDto
            {
                FileName    = f.FileName,
                RelativePath = f.RelativePath,
                Language    = f.Language,
                Layer       = layer,
                MatchReason = tokens.First(t => f.FileName.ToLower().Contains(t) || f.RelativePath.ToLower().Contains(t)),
            };
        }).ToList();

        // Layer aggregation
        var affectedLayers = affectedFiles
            .GroupBy(f => f.Layer)
            .Select(g =>
            {
                var rule = _layerRules.FirstOrDefault(r => r.Name == g.Key);
                var layerProjects = allProjects
                    .Where(p => g.Any(f => f.RelativePath.StartsWith(p.Directory.Replace("\\", "/"), StringComparison.OrdinalIgnoreCase)))
                    .Select(p => p.Name).Distinct().ToList();
                return new AffectedLayerDto
                {
                    Name      = g.Key,
                    Color     = rule.Color ?? "secondary",
                    Icon      = rule.Icon ?? "bi-layers",
                    FileCount = g.Count(),
                    Projects  = layerProjects,
                };
            })
            .OrderByDescending(l => l.FileCount)
            .ToList();

        // Risk scoring: 0-100
        int score = Math.Min(
            affectedFiles.Count * 5 +
            affectedLayers.Count * 10 +
            (affectedLayers.Any(l => l.Name is "API Layer" or "Domain / Core") ? 20 : 0),
            100);

        var (riskLevel, riskColor) = score switch
        {
            >= 70 => ("High",   "danger"),
            >= 40 => ("Medium", "warning"),
            _     => ("Low",    "success"),
        };

        var recommendations = BuildRecommendations(affectedLayers, affectedFiles.Count, changeType);

        // Request AI narrative
        string? narrative = null;
        try
        {
            var aiBase = _config["AiService:BaseUrl"] ?? "http://localhost:8000";
            var aiReq  = new
            {
                prompt = $"You are a senior software architect performing change impact analysis. A developer wants to {changeType} the component \"{component}\". Risk Level: {riskLevel}. Affected Files: {affectedFiles.Count}. Affected Layers: {string.Join(", ", affectedLayers.Select(l => l.Name))}. Recommendations: {string.Join("; ", recommendations)}. Write a concise impact narrative (3-5 sentences) that summarises what this change means for the system, highlights the most important risks, calls out cross-cutting concerns, and recommends a testing strategy.",
            };
            var client = _httpFactory.CreateClient();
            var body   = new StringContent(JsonSerializer.Serialize(aiReq), Encoding.UTF8, "application/json");
            var resp   = await client.PostAsync($"{aiBase}/api/gateway/complete/raw", body);
            if (resp.IsSuccessStatusCode)
            {
                var json   = await resp.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<JsonElement>(json);
                narrative  = parsed.TryGetProperty("content", out var c) ? c.GetString() : null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI narrative call failed for impact analysis");
        }

        var result = new ImpactAnalysisDto
        {
            RepositoryId        = repositoryId,
            RepositoryName      = repo.Name,
            Component           = component,
            ChangeType          = changeType,
            RiskScore           = score,
            RiskLevel           = riskLevel,
            RiskColor           = riskColor,
            DirectlyAffectedFiles = affectedFiles.Count,
            TotalAffectedFiles  = affectedFiles.Count,
            AffectedLayerCount  = affectedLayers.Count,
            AffectedProjectCount = affectedLayers.SelectMany(l => l.Projects).Distinct().Count(),
            AffectedFiles       = affectedFiles.Take(50).ToList(),
            AffectedLayers      = affectedLayers,
            Recommendations     = recommendations,
            AiNarrative         = narrative,
        };

        return Ok(result);
    }

    private string ClassifyLayer(string path, string fileName)
    {
        var combined = (path + "/" + fileName).ToLower();
        foreach (var (name, _, _, keywords) in _layerRules)
            if (keywords.Any(k => combined.Contains(k)))
                return name;
        return "General";
    }

    private static List<string> BuildRecommendations(List<AffectedLayerDto> layers, int fileCount, string changeType)
    {
        var recs = new List<string>();

        if (layers.Any(l => l.Name == "Domain / Core"))
            recs.Add("Review all interface contracts in ECIP.Core — downstream services depend on these abstractions.");

        if (layers.Any(l => l.Name == "Shared / DTOs"))
            recs.Add("Update all DTO consumers in ECIP.Web and ECIP.API to reflect any contract changes.");

        if (layers.Any(l => l.Name == "API Layer"))
            recs.Add("Run integration tests against all affected API endpoints.");

        if (layers.Any(l => l.Name == "Infrastructure"))
            recs.Add("Check EF Core migrations — schema changes may require a new migration.");

        if (fileCount > 10)
            recs.Add("High file count: consider a feature branch and a staged rollout.");

        if (changeType == "delete")
            recs.Add("Deleting a component: verify no remaining references with a project-wide search before removing.");

        recs.Add("Run the full test suite and review dependency injection registrations in Program.cs.");

        return recs;
    }
}
