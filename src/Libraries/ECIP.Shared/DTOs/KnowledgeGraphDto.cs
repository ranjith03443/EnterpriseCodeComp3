namespace ECIP.Shared.DTOs;

public class GraphNodeDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "secondary";
    public string Icon { get; set; } = "bi-circle";
    public Dictionary<string, string> Properties { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}

public class GraphEdgeDto
{
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class KnowledgeGraphDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public List<GraphNodeDto> Nodes { get; set; } = [];
    public List<GraphEdgeDto> Edges { get; set; } = [];
    public int TotalNodes => Nodes.Count;
    public int TotalEdges => Edges.Count;
    public List<string> NodeTypes { get; set; } = [];
}
