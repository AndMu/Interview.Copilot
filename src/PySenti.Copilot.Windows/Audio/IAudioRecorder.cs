namespace PySenti.Copilot.Windows.Audio;

public interface IAudioRecorder
{
    bool IsRecording { get; }
    string? ResultFile { get; }
    void Start(IAudioSelector source);
    Task<bool> Stop();
}