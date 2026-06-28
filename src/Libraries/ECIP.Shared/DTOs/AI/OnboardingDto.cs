namespace ECIP.Shared.DTOs.AI;

public class OnboardingRequestDto
{
    public Guid RepositoryId { get; set; }
    public string Query { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public List<SourceChunkDto> Chunks { get; set; } = [];
}

public class SourceChunkDto
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Score { get; set; }
}

public class OnboardingResponseDto
{
    public string Answer { get; set; } = string.Empty;
    public List<SourceChunkDto> Sources { get; set; } = [];
    public string Intent { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
}
