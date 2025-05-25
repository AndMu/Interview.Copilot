namespace PySenti.Copilot.Windows.Audio;

public interface IMp3Converter
{
    Task Convert(string inputWavFile, string outputMp3File);
}