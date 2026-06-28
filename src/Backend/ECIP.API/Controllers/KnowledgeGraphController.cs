using ECIP.Core.Interfaces;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/knowledgegraph")]
public class KnowledgeGraphController : ControllerBase
{
    private readonly IRepositoryService _repositories;
    private readonly IMetadataExtractionService _metadata;
    private readonly ILogger<KnowledgeGraphController> _logger;

    public KnowledgeGraphController(
        IRepositoryService repositories,
        IMetadataExtractionService metadata,
        ILogger<KnowledgeGraphController> logger)
    {
        _repositories = repositories;
        _metadata = metadata;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGraph(Guid id)
    {
        var repository = await _repositories.GetRepositoryAsync(id);
        if (repository is null)
            return NotFound(new ApiResponse<string>("Repository not found.", false));

        var projects  = await _metadata.GetProjectsAsync(id);
        var languages = await _metadata.GetLanguageSummaryAsync(id);
        var stats     = await _metadata.GetRepositoryStatisticsAsync(id);

        var nodes = new List<GraphNodeDto>();
        var edges = new List<GraphEdgeDto>();

        // Repository node
        var repoNodeId = $"repo-{id}";
        nodes.Add(new GraphNodeDto
        {
            Id          = repoNodeId,
            Label       = repository.Name,
            Type        = "Repository",
            Description = $"Root repository node — {repository.RepositoryType} repository",
            Color       = "primary",
            Icon        = "bi-box-seam",
            Properties  = new Dictionary<string, string>
            {
                ["Type"]   = repository.RepositoryType.ToString(),
                ["Status"] = repository.Status,
                ["Branch"] = repository.Branch ?? "main",
                ["Files"]  = (stats?.TotalFiles ?? 0).ToString(),
            },
            Tags = ["root", "repository"],
        });

        // Layer detection for project classification
        var layerKeywords = new Dictionary<string, (string Color, string Icon, string Label)>
        {
            ["web|ui|mvc|frontend|portal|blazor"] = ("primary",   "bi-window",        "Presentation"),
            ["api|gateway|rest|endpoint"]          = ("success",   "bi-hdd-network",   "API Layer"),
            ["core|domain|business|application"]   = ("warning",   "bi-bullseye",      "Domain/Core"),
            ["infrastructure|data|repository|persistence|ef"] = ("danger", "bi-database", "Infrastructure"),
            ["shared|common|contracts|dto|abstractions"]      = ("info",  "bi-share",      "Shared"),
            ["ai|worker|service|agent"]            = ("secondary", "bi-robot",         "AI/Services"),
            ["test|spec|fixture"]                  = ("dark",      "bi-check2-circle", "Tests"),
        };

        // Layer nodes
        var detectedLayers = new Dictionary<string, GraphNodeDto>();
        foreach (var p in projects)
        {
            foreach (var (pattern, (color, icon, label)) in layerKeywords)
            {
                if (pattern.Split('|').Any(k => p.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!detectedLayers.ContainsKey(label))
                    {
                        var layerNodeId = $"layer-{label.Replace("/", "-").Replace(" ", "-").ToLower()}";
                        var layerNode = new GraphNodeDto
                        {
                            Id          = layerNodeId,
                            Label       = label,
                            Type        = "Layer",
                            Description = $"Architectural layer grouping {label} projects",
                            Color       = color,
                            Icon        = icon,
                            Tags        = ["layer", "architecture"],
                        };
                        detectedLayers[label] = layerNode;
                        nodes.Add(layerNode);
                        edges.Add(new GraphEdgeDto { SourceId = repoNodeId, TargetId = layerNodeId, Label = "has layer", Type = "composition" });
                    }
                    break;
                }
            }
        }

        // Project nodes
        foreach (var p in projects)
        {
            var projectNodeId = $"project-{p.Id}";
            string assignedLayer = "Other";
            string projColor = "secondary";
            string projIcon  = "bi-box";

            foreach (var (pattern, (color, icon, label)) in layerKeywords)
            {
                if (pattern.Split('|').Any(k => p.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                {
                    assignedLayer = label;
                    projColor = color;
                    projIcon  = icon;
                    break;
                }
            }

            nodes.Add(new GraphNodeDto
            {
                Id          = projectNodeId,
                Label       = p.Name,
                Type        = "Project",
                Description = $"{p.ProjectType} project at {p.RelativePath}",
                Color       = projColor,
                Icon        = projIcon,
                Properties  = new Dictionary<string, string>
                {
                    ["ProjectType"] = p.ProjectType,
                    ["FileName"]    = p.FileName,
                    ["Layer"]       = assignedLayer,
                },
                Tags = ["project", p.ProjectType.ToLower().Replace(" ", "-"), assignedLayer.ToLower()],
            });

            edges.Add(new GraphEdgeDto { SourceId = repoNodeId, TargetId = projectNodeId, Label = "contains", Type = "composition" });

            if (detectedLayers.TryGetValue(assignedLayer, out var layerNode))
                edges.Add(new GraphEdgeDto { SourceId = projectNodeId, TargetId = layerNode.Id, Label = "belongs to", Type = "classification" });
        }

        // Language / Technology nodes
        foreach (var lang in languages)
        {
            var (color, icon) = GetLangStyle(lang.Language);
            var langNodeId = $"lang-{lang.Language.Replace("#", "sharp").Replace(" ", "-").Replace("/", "-").ToLower()}";
            nodes.Add(new GraphNodeDto
            {
                Id          = langNodeId,
                Label       = lang.Language,
                Type        = "Technology",
                Description = $"{lang.Language} — {lang.FileCount} files ({lang.Percentage}%)",
                Color       = color,
                Icon        = icon,
                Properties  = new Dictionary<string, string>
                {
                    ["Files"]      = lang.FileCount.ToString(),
                    ["Percentage"] = $"{lang.Percentage}%",
                    ["Extension"]  = lang.Extension,
                },
                Tags = ["technology", "language"],
            });
            edges.Add(new GraphEdgeDto { SourceId = repoNodeId, TargetId = langNodeId, Label = "uses", Type = "usage" });
        }

        var nodeTypes = nodes.Select(n => n.Type).Distinct().OrderBy(t => t).ToList();

        var graph = new KnowledgeGraphDto
        {
            RepositoryId   = id,
            RepositoryName = repository.Name,
            Nodes          = nodes,
            Edges          = edges,
            NodeTypes      = nodeTypes,
        };

        _logger.LogInformation(
            "Knowledge graph built for {Repo}: {Nodes} nodes, {Edges} edges",
            repository.Name, nodes.Count, edges.Count);

        return Ok(new ApiResponse<KnowledgeGraphDto>(graph, "Knowledge graph built successfully."));
    }

    private static (string Color, string Icon) GetLangStyle(string lang) => lang switch
    {
        "C#" or "Visual Basic" or "F#" => ("primary",   "bi-filetype-cs"),
        "TypeScript"                    => ("info",      "bi-filetype-tsx"),
        "JavaScript"                    => ("warning",   "bi-filetype-js"),
        "Python"                        => ("success",   "bi-filetype-py"),
        "HTML" or "CSS" or "SCSS"       => ("danger",    "bi-filetype-html"),
        "SQL"                           => ("secondary", "bi-database"),
        "YAML" or "JSON" or "XML"       => ("secondary", "bi-gear"),
        "Markdown"                      => ("dark",      "bi-file-text"),
        "PowerShell" or "Shell"         => ("dark",      "bi-terminal"),
        _                               => ("secondary", "bi-code-slash"),
    };
}
