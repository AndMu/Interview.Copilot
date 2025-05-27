namespace PySenti.Copilot.Prompts;

public interface IInterviewPromptGenerator
{
    InterviewPromptDefinition GeneratePrompt(InterviewPromptDefinition request);
}