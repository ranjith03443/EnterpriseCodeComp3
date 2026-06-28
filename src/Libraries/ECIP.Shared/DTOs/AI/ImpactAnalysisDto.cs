namespace ECIP.Shared.DTOs.AI;

public class ImpactAnalysisRequestDto
{
    public Guid RepositoryId { get; set; }
    public string Component { get; set; } = string.Empty;
    public string ChangeType { get; set; } = "modify";
}

public class AffectedFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Layer { get; set; } = string.Empty;
    public string MatchReason { get; set; } = string.Empty;
}

public class AffectedLayerDto
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public List<string> Projects { get; set; } = [];
}

public class ImpactAnalysisDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string RiskColor { get; set; } = string.Empty;
    public int DirectlyAffectedFiles { get; set; }
    public int TotalAffectedFiles { get; set; }
    public int AffectedLayerCount { get; set; }
    public int AffectedProjectCount { get; set; }
    public List<AffectedFileDto> AffectedFiles { get; set; } = [];
    public List<AffectedLayerDto> AffectedLayers { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];
    public string? AiNarrative { get; set; }
    public string AnalysedAt { get; set; } = DateTime.UtcNow.ToString("o");
}
