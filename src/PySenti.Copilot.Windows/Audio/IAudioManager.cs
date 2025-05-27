namespace PySenti.Copilot.Windows.Audio;

public interface IAudioManager
{
    IAudioSelector Input { get; }

    IAudioSelector Output { get; }

    IAudioRecorder Recorder { get; }
}