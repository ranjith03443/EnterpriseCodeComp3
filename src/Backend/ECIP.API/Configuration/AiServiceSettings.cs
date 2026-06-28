namespace ECIP.API.Configuration;

public class AiServiceSettings
{
    public string BaseUrl { get; set; } = "http://localhost:8000";
    public bool Enabled { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 30;
}
