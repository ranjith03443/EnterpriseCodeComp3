using System.Text;
using System.Text.Json;
using ECIP.Core.Interfaces;
using ECIP.Infrastructure.Persistence;
using ECIP.Shared.DTOs.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/codegen")]
public class CodeGeneratorController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly IRepositoryService _repositories;
    private readonly EcipDbContext _db;
    private readonly ILogger<CodeGeneratorController> _logger;

    public CodeGeneratorController(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        IRepositoryService repositories,
        EcipDbContext db,
        ILogger<CodeGeneratorController> logger)
    {
        _httpFactory = httpFactory;
        _config = config;
        _repositories = repositories;
        _db = db;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] CodeGenerationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Template))
            return BadRequest("Name and Template are required.");

        try
        {
            var aiBase = _config["AiService:BaseUrl"] ?? "http://localhost:8000";
            var client = _httpFactory.CreateClient();
            var body   = new StringContent(
                JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }),
                Encoding.UTF8, "application/json");
            var resp = await client.PostAsync($"{aiBase}/api/codegen/generate", body);
            if (!resp.IsSuccessStatusCode)
                return StatusCode((int)resp.StatusCode, "AI service error");

            var json   = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CodeGenerationResponseDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code generation failed");
            return StatusCode(503, "AI service unavailable");
        }
    }

    [HttpGet("templates")]
    public IActionResult GetTemplates()
    {
        var templates = new[]
        {
            new { Id="service",        Name="Service Class",      Icon="bi-gear",             Description="Business logic service with interface and DI" },
            new { Id="controller",     Name="API Controller",     Icon="bi-hdd-network",      Description="ASP.NET Core Web API controller with CRUD endpoints" },
            new { Id="dto",            Name="DTO / Model",        Icon="bi-box",              Description="Data transfer object or request/response model" },
            new { Id="repository",     Name="Repository",         Icon="bi-database",         Description="Data access repository with EF Core or Dapper" },
            new { Id="interface",      Name="Interface",          Icon="bi-diagram-3",        Description="C# interface definition with XML doc comments" },
            new { Id="test",           Name="Unit Test Class",    Icon="bi-check2-square",    Description="xUnit or NUnit test class with arrange/act/assert pattern" },
            new { Id="middleware",     Name="Middleware",         Icon="bi-arrow-left-right", Description="ASP.NET Core middleware for request pipeline" },
            new { Id="backgroundjob",  Name="Background Job",     Icon="bi-clock",            Description="IHostedService or BackgroundService implementation" },
        };
        return Ok(templates);
    }
}
