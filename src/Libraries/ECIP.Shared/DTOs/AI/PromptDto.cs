namespace ECIP.Shared.DTOs.AI;

public class PromptDto
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Parameters { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public class PromptDetailDto : PromptDto
{
    public string Template { get; set; } = string.Empty;
}
