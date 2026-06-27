namespace ECIP.Core.Interfaces.RepositoryIntelligence;

using ECIP.Core.Entities;

/// <summary>
/// Contract for discovering repository files and folders.
/// </summary>
public interface IFileDiscoveryService
{
    /// <summary>
    /// Discovers files and folders for the specified repository path.
    /// </summary>
    Task<(IReadOnlyList<RepositoryFile> Files, IReadOnlyList<RepositoryFolder> Folders)> DiscoverAsync(string repositoryPath, IReadOnlyList<string> ignoredFolders, long maximumFileSize);
}
