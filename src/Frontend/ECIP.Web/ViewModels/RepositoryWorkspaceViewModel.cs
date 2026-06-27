namespace ECIP.Web.ViewModels;

using ECIP.Core.Entities;

/// <summary>
/// View model for repository workspace management.
/// </summary>
public class RepositoryWorkspaceViewModel
{
    public List<RepositoryEntity> Repositories { get; set; } = new();
    public RepositoryEntity? SelectedRepository { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public bool ShowForm { get; set; }
    public string? FormMode { get; set; }
    public RepositoryEntity FormModel { get; set; } = new();
    public int TotalRepositories => Repositories.Count;
    public int GitHubCount => Repositories.Count(r => r.RepositoryType == ECIP.Core.Enums.RepositoryType.GitHub);
    public int AzureDevOpsCount => Repositories.Count(r => r.RepositoryType == ECIP.Core.Enums.RepositoryType.AzureDevOps);
    public int GitLabCount => Repositories.Count(r => r.RepositoryType == ECIP.Core.Enums.RepositoryType.GitLab);
    public int BitbucketCount => Repositories.Count(r => r.RepositoryType == ECIP.Core.Enums.RepositoryType.Bitbucket);
    public int LocalCount => Repositories.Count(r => r.RepositoryType == ECIP.Core.Enums.RepositoryType.LocalFolder);
    public string ActiveRepositoryName => Repositories.FirstOrDefault(r => r.IsActive)?.Name ?? "None";
}
