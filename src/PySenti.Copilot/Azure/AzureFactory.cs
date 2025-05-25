using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using PySenti.Copilot.Config;
using PySenti.Copilot.Definitions;
using PySenti.Copilot.Logic;

namespace PySenti.Copilot.Azure
{
    public class AzureFactory : ILlmFactory, ITranscriptionFactory
    {
        private readonly ILogger<AzureFactory> logger;

        public AzureFactory(ILogger<AzureFactory> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            this.logger = logger;
        }

        public ILlmService Create(AiModel model)
        {
            var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(model.Model, model.Endpoint, model.ApiKey);
            Kernel kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            return new LlmService(chatCompletionService);
        }

        ITranscriptionService ITranscriptionFactory.Create(AiModel model)
        {
#pragma warning disable SKEXP0010
            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIAudioToText(model.Model, model.Endpoint, model.ApiKey)
#pragma warning restore SKEXP0010
                .Build();

#pragma warning disable SKEXP0001
            var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();
            return new TranscriptionService(audioToTextService);
#pragma warning restore SKEXP0001
        }
    }
}
