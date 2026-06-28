namespace ECIP.Shared.DTOs.AI;

public class PlannerRequestDto
{
    public string Query { get; set; } = string.Empty;
    public string? Context { get; set; }
}

public class PlanStepDto
{
    public string StepId { get; set; } = string.Empty;
    public int StepNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PromptName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? Error { get; set; }
    public int? DurationMs { get; set; }
}

public class PlannerResponseDto
{
    public string PlanId { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string Intent { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<PlanStepDto> Steps { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int TotalDurationMs { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string? CompletedAt { get; set; }
}
