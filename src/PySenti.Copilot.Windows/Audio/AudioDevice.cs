namespace PySenti.Copilot.Windows.Audio
{
    public record AudioDevice
    {
        public int Id { get; init; } = 0;

        public string Name { get; init; } = null!;
    }
}
