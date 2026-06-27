namespace ECIP.Core.Entities;

/// <summary>
/// Represents a language detected for repository files.
/// </summary>
public class RepositoryLanguage
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public string Extension { get; set; } = string.Empty;
}
