namespace ECIP.Core.Interfaces;

using ECIP.Core.Entities;

/// <summary>
/// Service contract for repository workspace operations.
/// </summary>
public interface IRepositoryService
{
    Task<IReadOnlyList<RepositoryEntity>> GetRepositoriesAsync();
    Task<RepositoryEntity?> GetRepositoryAsync(Guid id);
    Task<RepositoryEntity> AddRepositoryAsync(RepositoryEntity repository);
    Task<RepositoryEntity?> UpdateRepositoryAsync(RepositoryEntity repository);
    Task<bool> DeleteRepositoryAsync(Guid id);
    Task<RepositoryEntity?> SetActiveRepositoryAsync(Guid id);
    Task<RepositoryEntity?> CloneRepositoryAsync(string repositoryUrl, string localPath, string branch = "main");
    Task<RepositoryEntity?> RegisterLocalRepositoryAsync(string name, string localPath, string description = "");
}
