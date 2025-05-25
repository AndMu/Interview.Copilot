using Microsoft.Extensions.Logging;
using PySenti.Copilot.Config;

namespace PySenti.Copilot.Definitions
{
    public class CopilotService : ICopilotService
    {
        private readonly ILogger<CopilotService> logger;        

        private readonly ILlmFactory llmFactory;

        private readonly ITranscriptionFactory transcriptionFactory;

        private CopilotConfig? copilotConfig;        

        public CopilotService(
            ILogger<CopilotService> logger, 
            ICopilotConfigService configService, 
            ILlmFactory llmFactory,
            ITranscriptionFactory transcriptionFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configService);
            ArgumentNullException.ThrowIfNull(llmFactory);
            ArgumentNullException.ThrowIfNull(transcriptionFactory);
            this.ConfigService = configService;
            this.llmFactory = llmFactory;
            this.transcriptionFactory = transcriptionFactory;
            this.logger = logger;
        }

        public ICopilotConfigService ConfigService { get; }

        public string[] Llms { get; private set; } = [];

        public string[] Transcriptions { get; private set; } = [];

        public async Task Refresh()
        {
            logger.LogDebug("Refreshing configuration");
            copilotConfig = await ConfigService.Load();
            Llms = copilotConfig.LlmModels.Select(item => item.Model).ToArray();
            Transcriptions = copilotConfig.TranscriptionModels.Select(item => item.Model).ToArray();
        }

        public ILlmService Resolve(string name)
        {
            logger.LogDebug("Resolving LLM service for {name}", name);
            var model = copilotConfig?.LlmModels.FirstOrDefault(item => item.Model == name);
            if (model == null)
            {
                logger.LogWarning("LLM model {name} not found", name);
                throw new ArgumentException($"LLM model {name} not found");
            }

            return llmFactory.Create(model);
        }

        public ITranscriptionService ResolveTranscription(string name)
        {
            logger.LogDebug("Resolving transcription service for {name}", name);
            var model = copilotConfig?.TranscriptionModels.FirstOrDefault(item => item.Model == name);
            if (model == null)
            {
                logger.LogWarning("Transcription model {name} not found", name);
                throw new ArgumentException($"Transcription model {name} not found");
            }

            return transcriptionFactory.Create(model);
        }
    }
}
