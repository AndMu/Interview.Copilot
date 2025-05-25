namespace PySenti.Copilot.Prompts
{
    public record AllPrompts
    {
        public List<InterviewPromptDefinition> Definitions { get; init; } = new();

        public string? DefaultPrompt { get; set; }
    }
}
