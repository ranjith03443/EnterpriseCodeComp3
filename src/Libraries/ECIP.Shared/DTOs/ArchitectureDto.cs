namespace ECIP.Shared.DTOs;

public class ArchitectureLayerDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "secondary";
    public string Icon { get; set; } = "bi-layers";
    public List<ProjectInLayerDto> Projects { get; set; } = [];
}

public class ProjectInLayerDto
{
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
}

public class TechStackItemDto
{
    public string Technology { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Color { get; set; } = "secondary";
    public string Icon { get; set; } = "bi-code";
    public int FileCount { get; set; }
    public double Percentage { get; set; }
}

public class ArchitectureViewDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string ArchitectureStyle { get; set; } = string.Empty;
    public List<ArchitectureLayerDto> Layers { get; set; } = [];
    public List<TechStackItemDto> TechStack { get; set; } = [];
    public int TotalProjects { get; set; }
    public int ClassifiedProjects { get; set; }
    public int UnclassifiedProjects { get; set; }
}
