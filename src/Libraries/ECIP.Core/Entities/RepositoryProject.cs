namespace ECIP.Core.Entities;

/// <summary>
/// Represents a discovered project within a repository (e.g., .sln, .csproj, package.json).
/// </summary>
public class RepositoryProject
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public DateTime DiscoveredDate { get; set; } = DateTime.UtcNow;
}
