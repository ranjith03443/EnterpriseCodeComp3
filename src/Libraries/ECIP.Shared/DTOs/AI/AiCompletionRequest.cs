namespace ECIP.Shared.DTOs.AI;

public class AiCompletionRequest
{
    public string PromptName { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public string System { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
}

public class AiRawCompletionRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
}
