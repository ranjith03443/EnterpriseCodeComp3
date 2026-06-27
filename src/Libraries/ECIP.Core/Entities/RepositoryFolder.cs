namespace ECIP.Core.Entities;

/// <summary>
/// Represents metadata for a discovered folder in a repository.
/// </summary>
public class RepositoryFolder
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string ParentPath { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
