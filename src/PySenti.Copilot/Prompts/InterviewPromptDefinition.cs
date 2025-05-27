namespace PySenti.Copilot.Prompts
{
    public class InterviewPromptDefinition // it can't be record - bindings will not work properly
    {
        public string Role { get; init; } = string.Empty;

        public string Short { get; init; }

        public string Long { get; init; }
    }
}
