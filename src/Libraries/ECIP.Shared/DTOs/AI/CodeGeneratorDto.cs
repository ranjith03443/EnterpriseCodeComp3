namespace ECIP.Shared.DTOs.AI;

public class CodeGenerationRequestDto
{
    public string Template   { get; set; } = string.Empty;
    public string Language   { get; set; } = "C#";
    public string Name       { get; set; } = string.Empty;
    public string Description{ get; set; } = string.Empty;
    public string Context    { get; set; } = string.Empty;
    public List<string> Features { get; set; } = [];
}

public class CodeGenerationResponseDto
{
    public string Code       { get; set; } = string.Empty;
    public string Language   { get; set; } = string.Empty;
    public string Template   { get; set; } = string.Empty;
    public string Name       { get; set; } = string.Empty;
    public List<string> Files{ get; set; } = [];
    public string Explanation{ get; set; } = string.Empty;
    public int DurationMs    { get; set; }
    public string CreatedAt  { get; set; } = DateTime.UtcNow.ToString("o");
}
