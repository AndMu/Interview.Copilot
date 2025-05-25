namespace PySenti.Copilot.Prompts
{
    public record PromptDefinition
    {
        public string Prompt { get; init; } = string.Empty;

        public PromptType Type { get; init; } = PromptType.Short;
    }
}
