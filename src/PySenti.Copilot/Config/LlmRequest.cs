namespace PySenti.Copilot.Config
{
    public record LlmRequest
    {
        public string System { get; init; } = null!;

        public string Question { get; init; } = null!;

        public byte[]? ImageData { get; init; } = null!;
    }
}
