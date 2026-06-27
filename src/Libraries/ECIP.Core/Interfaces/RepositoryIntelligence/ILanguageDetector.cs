namespace ECIP.Core.Interfaces.RepositoryIntelligence;

using ECIP.Core.Entities;

/// <summary>
/// Contract for detecting repository file languages from file extensions.
/// </summary>
public interface ILanguageDetector
{
    /// <summary>
    /// Detects the language for a file.
    /// </summary>
    RepositoryLanguage Detect(string fileName);
}
