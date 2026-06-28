namespace ECIP.Web.ViewModels;

public class PlannerViewModel
{
    public string Query { get; set; } = string.Empty;
    public string? Context { get; set; }
    public bool AiServiceAvailable { get; set; }
}
