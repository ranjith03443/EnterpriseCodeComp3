namespace ECIP.Shared.DTOs;

public class FlowStepDto
{
    public int Order { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Layer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-arrow-right-circle";
    public string Color { get; set; } = "primary";
    public List<string> Components { get; set; } = [];
}

public class FlowDefinitionDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = "bi-bezier2";
    public string Color { get; set; } = "primary";
    public List<FlowStepDto> Steps { get; set; } = [];
}

public class FlowViewDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string ArchitectureStyle { get; set; } = string.Empty;
    public List<FlowDefinitionDto> Flows { get; set; } = [];
}
