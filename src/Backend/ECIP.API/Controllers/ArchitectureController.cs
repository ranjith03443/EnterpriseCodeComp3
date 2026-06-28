using ECIP.Core.Interfaces;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/architecture")]
public class ArchitectureController : ControllerBase
{
    private readonly IRepositoryService _repositories;
    private readonly IMetadataExtractionService _metadata;
    private readonly ILogger<ArchitectureController> _logger;

    private static readonly List<(string Name, string Description, string Color, string Icon, string[] Keywords)> _layerRules =
    [
        ("Presentation", "UI and user-facing components", "primary",   "bi-window",         ["web", "ui", "frontend", "mvc", "blazor", "razor", "portal", "client"]),
        ("API Layer",    "HTTP API and gateway endpoints", "success",   "bi-hdd-network",    ["api", "gateway", "rest", "endpoint", "graphql", "grpc"]),
        ("Domain / Core","Business logic and domain model","warning",   "bi-bullseye",       ["core", "domain", "business", "application", "model", "entity", "usecase"]),
        ("Infrastructure","Data access and external services","danger", "bi-database",       ["infrastructure", "data", "repository", "persistence", "dal", "ef", "sqlite", "mongo", "redis"]),
        ("Shared / Common","Cross-cutting contracts and utilities","info","bi-share",        ["shared", "common", "contracts", "dto", "abstractions", "utilities", "helpers"]),
        ("AI / Services","AI microservices and background workers","secondary","bi-robot",   ["ai", "worker", "background", "jobs", "service", "agent", "ml"]),
        ("Tests",        "Test projects and test utilities", "dark",    "bi-check2-circle",  ["test", "spec", "fixture", "integration", "unit", "e2e"]),
    ];

    private static readonly Dictionary<string, (string Role, string Color, string Icon)> _techMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["C#"]                      = ("Backend (.NET)",        "primary",   "bi-filetype-cs"),
        ["Visual Basic"]            = ("Backend (.NET)",        "primary",   "bi-filetype-cs"),
        ["F#"]                      = ("Backend (.NET)",        "primary",   "bi-filetype-cs"),
        ["TypeScript"]              = ("Frontend / Node",       "info",      "bi-filetype-tsx"),
        ["JavaScript"]              = ("Frontend / Node",       "warning",   "bi-filetype-js"),
        ["Python"]                  = ("AI / Scripting",        "success",   "bi-filetype-py"),
        ["HTML"]                    = ("Frontend Templates",    "danger",    "bi-filetype-html"),
        ["CSS"]                     = ("Styling",               "info",      "bi-filetype-css"),
        ["SCSS"]                    = ("Styling",               "info",      "bi-filetype-css"),
        ["SQL"]                     = ("Database",              "secondary", "bi-database"),
        ["YAML"]                    = ("Configuration",         "secondary", "bi-gear"),
        ["JSON"]                    = ("Configuration / Data",  "secondary", "bi-braces"),
        ["XML"]                     = ("Configuration",         "secondary", "bi-code-slash"),
        ["Markdown"]                = ("Documentation",         "dark",      "bi-file-text"),
        ["PowerShell"]              = ("Automation",            "primary",   "bi-terminal"),
        ["Shell"]                   = ("Automation",            "secondary", "bi-terminal"),
        ["MSBuild"]                 = ("Build",                 "dark",      "bi-hammer"),
        ["Visual Studio Solution"]  = ("Build",                 "dark",      "bi-hammer"),
    };

    public ArchitectureController(
        IRepositoryService repositories,
        IMetadataExtractionService metadata,
        ILogger<ArchitectureController> logger)
    {
        _repositories = repositories;
        _metadata = metadata;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetArchitecture(Guid id)
    {
        var repository = await _repositories.GetRepositoryAsync(id);
        if (repository is null)
            return NotFound(new ApiResponse<string>("Repository not found.", false));

        var projects = await _metadata.GetProjectsAsync(id);
        var languages = await _metadata.GetLanguageSummaryAsync(id);

        var layers = BuildLayers(projects.Select(p => new ProjectInLayerDto
        {
            Name = p.Name,
            ProjectType = p.ProjectType,
            FileName = p.FileName,
            RelativePath = p.RelativePath,
            Directory = p.Directory,
        }).ToList());

        var techStack = BuildTechStack(languages.Select(l => new LanguageSummaryDto
        {
            Language = l.Language,
            FileCount = l.FileCount,
            Percentage = l.Percentage,
        }).ToList());

        var classifiedCount = layers.Sum(l => l.Projects.Count);
        var style = DetectArchitectureStyle(layers);

        var view = new ArchitectureViewDto
        {
            RepositoryId = id,
            RepositoryName = repository.Name,
            ArchitectureStyle = style,
            Layers = layers,
            TechStack = techStack,
            TotalProjects = projects.Count,
            ClassifiedProjects = classifiedCount,
            UnclassifiedProjects = projects.Count - classifiedCount,
        };

        _logger.LogInformation("Architecture loaded for {Repo}: {Style}, {Layers} layers, {Projects} projects",
            repository.Name, style, layers.Count, projects.Count);

        return Ok(new ApiResponse<ArchitectureViewDto>(view, "Architecture analysis complete."));
    }

    private static List<ArchitectureLayerDto> BuildLayers(List<ProjectInLayerDto> projects)
    {
        var assigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var layers = new List<ArchitectureLayerDto>();

        foreach (var (name, desc, color, icon, keywords) in _layerRules)
        {
            var matched = projects
                .Where(p => !assigned.Contains(p.Name) && MatchesLayer(p, keywords))
                .ToList();

            if (matched.Count == 0) continue;

            foreach (var p in matched) assigned.Add(p.Name);
            layers.Add(new ArchitectureLayerDto
            {
                Name = name,
                Description = desc,
                Color = color,
                Icon = icon,
                Projects = matched,
            });
        }

        var unclassified = projects.Where(p => !assigned.Contains(p.Name)).ToList();
        if (unclassified.Count > 0)
        {
            layers.Add(new ArchitectureLayerDto
            {
                Name = "Other",
                Description = "Projects not matched to a specific layer",
                Color = "light",
                Icon = "bi-question-circle",
                Projects = unclassified,
            });
        }

        return layers;
    }

    private static bool MatchesLayer(ProjectInLayerDto project, string[] keywords)
    {
        var name = project.Name.ToLower();
        var dir  = project.Directory.ToLower();
        return keywords.Any(k => name.Contains(k) || dir.Contains(k));
    }

    private static List<TechStackItemDto> BuildTechStack(List<LanguageSummaryDto> languages)
    {
        return languages
            .Where(l => _techMap.ContainsKey(l.Language))
            .Select(l =>
            {
                var (role, color, icon) = _techMap[l.Language];
                return new TechStackItemDto
                {
                    Technology = l.Language,
                    Role = role,
                    Color = color,
                    Icon = icon,
                    FileCount = l.FileCount,
                    Percentage = l.Percentage,
                };
            })
            .OrderByDescending(t => t.FileCount)
            .ToList();
    }

    private static string DetectArchitectureStyle(List<ArchitectureLayerDto> layers)
    {
        var names = layers.Select(l => l.Name).ToHashSet();
        if (names.Contains("Domain / Core") && names.Contains("Infrastructure") && names.Contains("Presentation"))
            return "Clean Architecture";
        if (names.Contains("Presentation") && names.Contains("API Layer") && names.Contains("Infrastructure"))
            return "N-Tier / Layered Architecture";
        if (names.Contains("AI / Services") && layers.Count >= 3)
            return "Microservices / Service-Oriented";
        return "Modular Architecture";
    }
}
