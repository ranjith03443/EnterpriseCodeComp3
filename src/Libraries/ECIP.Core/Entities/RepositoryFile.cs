namespace ECIP.Core.Entities;

/// <summary>
/// Represents metadata for a discovered file in a repository.
/// </summary>
public class RepositoryFile
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string Language { get; set; } = "Unknown";
}
