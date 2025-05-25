using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions;

public interface ITranscriptionService
{
    Task<string> TranscribeAudioAsync(string audioFilePath, Language language, CancellationToken token);
}