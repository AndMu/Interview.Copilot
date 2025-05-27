using NAudio.Wave;

namespace PySenti.Copilot.Windows.Audio;

public interface IAudioSelector
{
    IEnumerable<AudioDevice> Devices { get; }

    AudioDevice? Selected { get; set; }

    void LoadDevices();

    IWaveIn Aquire();
}