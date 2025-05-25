namespace PySenti.Copilot.Prompts;

public interface IInterviewPromptConfigurator
{
    AllPrompts? Prompts { get; set; }

    Task Load();

    Task Save();

}