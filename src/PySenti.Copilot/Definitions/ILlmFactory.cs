using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions
{
    public interface ILlmFactory
    {
        ILlmService Create(AiModel model);
    }
}
