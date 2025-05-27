using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions;

public interface ILlmService
{
    IAsyncEnumerable<string> GetAnswer(LlmRequest request, CancellationToken token);
}