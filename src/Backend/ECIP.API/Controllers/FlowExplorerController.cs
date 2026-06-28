using ECIP.Core.Interfaces;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/flow")]
public class FlowExplorerController : ControllerBase
{
    private readonly IRepositoryService _repositories;
    private readonly IMetadataExtractionService _metadata;
    private readonly ILogger<FlowExplorerController> _logger;

    public FlowExplorerController(
        IRepositoryService repositories,
        IMetadataExtractionService metadata,
        ILogger<FlowExplorerController> logger)
    {
        _repositories = repositories;
        _metadata = metadata;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFlows(Guid id)
    {
        var repository = await _repositories.GetRepositoryAsync(id);
        if (repository is null)
            return NotFound(new ApiResponse<string>("Repository not found.", false));

        var projects = await _metadata.GetProjectsAsync(id);
        var languages = await _metadata.GetLanguageSummaryAsync(id);

        var projectNames = projects.Select(p => p.Name).ToList();
        var langNames = languages.Select(l => l.Language).ToList();

        var flows = new List<FlowDefinitionDto>
        {
            BuildHttpRequestFlow(projectNames),
            BuildAiQueryFlow(),
            BuildRepositoryIntelligenceFlow(projectNames),
            BuildDataPersistenceFlow(projectNames, langNames),
        };

        var view = new FlowViewDto
        {
            RepositoryId = id,
            RepositoryName = repository.Name,
            ArchitectureStyle = DetectStyle(projectNames),
            Flows = flows,
        };

        _logger.LogInformation("Flow Explorer loaded {Count} flows for {Repo}", flows.Count, repository.Name);
        return Ok(new ApiResponse<FlowViewDto>(view, $"{flows.Count} execution flows discovered."));
    }

    private static FlowDefinitionDto BuildHttpRequestFlow(List<string> projectNames)
    {
        var presentationProjects = projectNames.Where(n => ContainsAny(n, "web", "ui", "mvc", "frontend", "portal")).ToList();
        var apiProjects          = projectNames.Where(n => ContainsAny(n, "api", "gateway", "rest")).ToList();
        var coreProjects         = projectNames.Where(n => ContainsAny(n, "core", "domain", "business")).ToList();
        var infraProjects        = projectNames.Where(n => ContainsAny(n, "infrastructure", "data", "persistence")).ToList();

        return new FlowDefinitionDto
        {
            Id = "http-request",
            Name = "HTTP Request Flow",
            Description = "Traces a user HTTP request from the browser through the application layers to the database and back.",
            Category = "API",
            Icon = "bi-hdd-network",
            Color = "primary",
            Steps =
            [
                new() { Order=1, Name="Client Browser",         Layer="External",         Icon="bi-browser-chrome", Color="secondary", Description="User initiates an HTTP request from the browser.",                                           Components=["Browser","HTTP Client","Fetch API"] },
                new() { Order=2, Name="Presentation Layer",     Layer="Presentation",     Icon="bi-window",         Color="primary",   Description="MVC controller receives the request, validates input, and calls the service layer.",        Components=presentationProjects.Any() ? presentationProjects : ["Web","MVC Controller","Razor View"] },
                new() { Order=3, Name="API / Gateway Layer",    Layer="API",              Icon="bi-hdd-network",    Color="success",   Description="Web API controller exposes the endpoint, maps DTOs, and delegates to the domain layer.",    Components=apiProjects.Any() ? apiProjects : ["API Controller","Route Handler","DTO Mapper"] },
                new() { Order=4, Name="Domain / Core Layer",    Layer="Domain",           Icon="bi-bullseye",       Color="warning",   Description="Business logic executes, applies domain rules, and orchestrates repository calls.",          Components=coreProjects.Any() ? coreProjects : ["Service","Domain Model","Business Rule"] },
                new() { Order=5, Name="Infrastructure Layer",   Layer="Infrastructure",   Icon="bi-database",       Color="danger",    Description="Repository implementations query the database and return mapped entities.",                   Components=infraProjects.Any() ? infraProjects : ["Repository","EF Core DbContext","SQLite"] },
                new() { Order=6, Name="Database",               Layer="External",         Icon="bi-server",         Color="dark",      Description="Persistent storage returns data to the repository layer.",                                   Components=["SQLite","Database Table","Query Result"] },
            ],
        };
    }

    private static FlowDefinitionDto BuildAiQueryFlow()
    {
        return new FlowDefinitionDto
        {
            Id = "ai-query",
            Name = "AI Query Flow",
            Description = "Traces a user question through the Planner Agent, AI Gateway, Prompt Registry, and LLM provider.",
            Category = "AI",
            Icon = "bi-robot",
            Color = "info",
            Steps =
            [
                new() { Order=1, Name="User Query",          Layer="Presentation", Icon="bi-chat-left-text", Color="primary",   Description="User submits a natural-language question via the Planner Agent UI.",                              Components=["Planner Page","/Planner/Ask","AJAX POST"] },
                new() { Order=2, Name="Intent Detection",    Layer="Planner",      Icon="bi-crosshair",      Color="info",      Description="IntentDetector classifies the query into a known intent using keyword pattern matching.",         Components=["IntentDetector","Regex Patterns","IntentResult"] },
                new() { Order=3, Name="Plan Building",       Layer="Planner",      Icon="bi-list-check",     Color="info",      Description="PlanBuilder maps the detected intent to an ordered list of prompt-driven execution steps.",       Components=["PlanBuilder","Plan Templates","ExecutionPlan"] },
                new() { Order=4, Name="Prompt Registry",     Layer="AI Service",   Icon="bi-journals",       Color="warning",   Description="The relevant prompt YAML is loaded, rendered with parameters, and passed to the gateway.",       Components=["PromptRegistry","YAML Prompts","render()"] },
                new() { Order=5, Name="AI Gateway",          Layer="AI Service",   Icon="bi-hdd-network",    Color="success",   Description="Gateway routes the rendered prompt to the active LLM provider with retry and backoff logic.",    Components=["AiGateway","RetryPolicy","LLM Router"] },
                new() { Order=6, Name="LLM Provider",        Layer="External",     Icon="bi-stars",          Color="secondary", Description="The configured LLM (OpenAI, Gemini, Ollama, or Mock) processes the prompt and returns a response.",Components=["Mock/OpenAI/Gemini/Ollama","Token Counter"] },
                new() { Order=7, Name="Plan Executor",       Layer="Planner",      Icon="bi-play-circle",    Color="info",      Description="PlanExecutor records step results, resolves cross-step references, and aggregates the summary.",  Components=["PlanExecutor","StepResult","Summary"] },
                new() { Order=8, Name="Response to User",    Layer="Presentation", Icon="bi-check-circle",   Color="primary",   Description="The execution plan with step results and final summary is returned and rendered in the UI.",      Components=["ExecutionPlan JSON","Accordion UI","Summary Card"] },
            ],
        };
    }

    private static FlowDefinitionDto BuildRepositoryIntelligenceFlow(List<string> projectNames)
    {
        var scannerProjects = projectNames.Where(n => ContainsAny(n, "infrastructure", "scanner", "intelligence")).ToList();

        return new FlowDefinitionDto
        {
            Id = "repo-intelligence",
            Name = "Repository Intelligence Flow",
            Description = "Traces how a repository is scanned, indexed, and its metadata extracted and stored.",
            Category = "Intelligence",
            Icon = "bi-graph-up-arrow",
            Color = "success",
            Steps =
            [
                new() { Order=1, Name="Trigger Scan",          Layer="Presentation", Icon="bi-play-btn",        Color="primary",   Description="User triggers a repository scan from the Repository Intelligence page.",                        Components=["Scan Button","POST /scan","RepositoryIntelligenceController"] },
                new() { Order=2, Name="Repository Scanner",    Layer="Service",      Icon="bi-search",          Color="success",   Description="RepositoryScannerService orchestrates the discovery process and updates repository status.",    Components=scannerProjects.Any() ? scannerProjects : ["RepositoryScannerService","IRepositoryScanner"] },
                new() { Order=3, Name="File Discovery",        Layer="Service",      Icon="bi-folder2-open",    Color="success",   Description="FileDiscoveryService recursively walks the directory tree, skipping ignored folders.",          Components=["FileDiscoveryService","Directory.GetFiles","Ignored Folders"] },
                new() { Order=4, Name="Language Detection",    Layer="Service",      Icon="bi-code-slash",      Color="info",      Description="LanguageDetector maps each file extension to a programming language name.",                      Components=["LanguageDetector","Extension Map","RepositoryLanguage"] },
                new() { Order=5, Name="Metadata Extraction",   Layer="Service",      Icon="bi-tags",            Color="warning",   Description="MetadataExtractionService computes statistics, discovers projects, and stores language data.",   Components=["MetadataExtractionService","RepositoryStatistics","RepositoryProject"] },
                new() { Order=6, Name="Persistence",           Layer="Infrastructure",Icon="bi-database-fill-check",Color="danger", Description="Results are persisted to the SQLite database via Entity Framework Core.",                     Components=["EcipDbContext","SaveChangesAsync","SQLite"] },
            ],
        };
    }

    private static FlowDefinitionDto BuildDataPersistenceFlow(List<string> projectNames, List<string> languages)
    {
        var usesEF      = projectNames.Any(n => ContainsAny(n, "infrastructure", "data", "ef")) || languages.Contains("C#");
        var usesSqlite  = true;

        return new FlowDefinitionDto
        {
            Id = "data-persistence",
            Name = "Data Persistence Flow",
            Description = "Shows how domain entities are mapped and persisted through the repository pattern to the database.",
            Category = "Data",
            Icon = "bi-database",
            Color = "warning",
            Steps =
            [
                new() { Order=1, Name="Domain Entity",       Layer="Domain",         Icon="bi-box",              Color="warning",   Description="A domain entity (e.g. RepositoryEntity) is created or updated in the domain/service layer.",     Components=["RepositoryEntity","DomainModel","POCO"] },
                new() { Order=2, Name="Repository Interface",Layer="Domain",         Icon="bi-plug",             Color="warning",   Description="The service calls an IRepository interface method, decoupling business logic from data access.",  Components=["IRepositoryService","Interface","DI Container"] },
                new() { Order=3, Name="Repository Impl",     Layer="Infrastructure", Icon="bi-gear",             Color="danger",    Description="The concrete repository implementation uses EF Core to translate domain calls to SQL queries.",    Components=["RepositoryService","EF Core","LINQ"] },
                new() { Order=4, Name="DbContext",           Layer="Infrastructure", Icon="bi-database",         Color="danger",    Description="EcipDbContext tracks entity state changes and coordinates the unit-of-work pattern.",             Components=usesEF ? ["EcipDbContext","DbSet<T>","ChangeTracker"] : ["DbContext","ORM"] },
                new() { Order=5, Name="SQLite Database",     Layer="External",       Icon="bi-server",           Color="dark",      Description=usesSqlite ? "SQLite stores the data in a local file, ideal for development and demos." : "The relational database executes the SQL and returns results.", Components=["SQLite","ecip.db","SQL Query"] },
            ],
        };
    }

    private static string DetectStyle(List<string> projectNames)
    {
        var hasCore   = projectNames.Any(n => ContainsAny(n, "core", "domain"));
        var hasInfra  = projectNames.Any(n => ContainsAny(n, "infrastructure", "data"));
        var hasWeb    = projectNames.Any(n => ContainsAny(n, "web", "ui", "mvc"));
        if (hasCore && hasInfra && hasWeb) return "Clean Architecture";
        if (hasWeb && hasInfra)            return "N-Tier Layered";
        return "Modular";
    }

    private static bool ContainsAny(string name, params string[] keywords)
        => keywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase));
}
