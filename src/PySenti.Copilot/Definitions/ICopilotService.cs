using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions
{
    public interface ICopilotService
    {
        ILlmService Resolve(string name);

        ITranscriptionService ResolveTranscription(string name);

        Task Refresh();

        ICopilotConfigService ConfigService { get; }

        string[] Llms { get; }

        string[] Transcriptions { get; }
    }
}
