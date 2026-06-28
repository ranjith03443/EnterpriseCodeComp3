namespace ECIP.Shared.DTOs.AI;

public class AiCompletionResponse
{
    public string Content { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int DurationMs { get; set; }
}

public class AiEmbeddingRequest
{
    public string Text { get; set; } = string.Empty;
}

public class AiEmbeddingResponse
{
    public List<double> Vector { get; set; } = new();
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Dimensions { get; set; }
    public int DurationMs { get; set; }
}
