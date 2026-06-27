namespace ECIP.Core.Entities;

/// <summary>
/// Represents statistics for a specific folder within a repository.
/// </summary>
public class RepositoryFolderStatistics
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public string ParentFolder { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public int NumberOfFiles { get; set; }
    public long TotalSize { get; set; }
    public int Depth { get; set; }
}
