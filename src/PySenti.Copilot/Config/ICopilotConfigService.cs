namespace PySenti.Copilot.Config;

public interface ICopilotConfigService
{
    string Location { get; }

    CopilotConfig? Active { get; }

    Task<CopilotConfig> Load();

    Task Save(CopilotConfig config);
}