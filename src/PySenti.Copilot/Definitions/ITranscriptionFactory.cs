using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions
{
    public interface ITranscriptionFactory
    {
        ITranscriptionService Create(AiModel model);
    }
}
