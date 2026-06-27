namespace ECIP.Core.Entities;

using ECIP.Core.Enums;

/// <summary>
/// Represents a repository registered in the workspace.
/// </summary>
public class RepositoryEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public RepositoryType RepositoryType { get; set; }
    public string Branch { get; set; } = "main";
    public string LocalPath { get; set; } = string.Empty;
    public string DefaultBranch { get; set; } = "main";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Registered";
    public DateTime? LastScanDate { get; set; }
    public bool IsActive { get; set; }
}
