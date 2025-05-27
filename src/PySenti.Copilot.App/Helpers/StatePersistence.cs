using Microsoft.Extensions.Logging;
using PySenti.Copilot.App.ViewModels;
using PySenti.Copilot.Config;

namespace PySenti.Copilot.App.Helpers
{
    public class StatePersistence : IStatePersistence
    {
        private readonly ILogger<StatePersistence> logger;

        private readonly ICopilotConfigService configService;

        public StatePersistence(
            ILogger<StatePersistence> logger,
            ICopilotConfigService configService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configService);
            this.configService = configService;
            this.logger = logger;
        }

        public async Task<bool> Save(MainWindowViewModel viewModel)
        {
            logger.LogDebug("Saving state...");
            try
            {
                var config = configService.Active;
                if (config != null)
                {
                    config.ShortLlm = viewModel.SelectedShortLlm;
                    config.LongLlm = viewModel.SelectedLongLlm;
                    config.Transcription = viewModel.SelectedTranscription;
                    config.Language = viewModel.SelectedLanguage;
                    
                    if (viewModel.SelectedAudioDevice != null)
                    {
                        config.SelectedAudioDevice = viewModel.SelectedAudioDevice.Name;
                    }

                    config.IsOutputAudio = viewModel.IsOutputSource;
                    config.SelectedPromptRole = viewModel.SelectedPrompt?.Role;
                    await configService.Save(config);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving state.");
            }
            
            return false;
        }
    }
}
