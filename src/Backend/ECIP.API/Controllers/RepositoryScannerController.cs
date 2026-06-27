using ECIP.Core.Entities;
using ECIP.Core.Interfaces.RepositoryIntelligence;
using ECIP.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/repositories")]
public class RepositoryScannerController : ControllerBase
{
    private readonly IRepositoryScanner _repositoryScanner;
    private readonly ILogger<RepositoryScannerController> _logger;

    public RepositoryScannerController(IRepositoryScanner repositoryScanner, ILogger<RepositoryScannerController> logger)
    {
        _repositoryScanner = repositoryScanner;
        _logger = logger;
    }

    [HttpPost("{repositoryId:guid}/scan")]
    public async Task<IActionResult> ScanRepository(Guid repositoryId)
    {
        try
        {
            var result = await _repositoryScanner.ScanRepositoryAsync(repositoryId);
            return Ok(new ApiResponse<RepositoryScanResult>(result, "Repository scan completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scan failed for repository {RepositoryId}", repositoryId);
            return BadRequest(new ApiResponse<string>(ex.Message, false));
        }
    }
}
