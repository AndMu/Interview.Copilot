using PySenti.Copilot.Config;

namespace PySenti.Copilot.App.ViewModels.Config
{
    public class TranscriptionConfigViewModel(ICopilotConfigService configService) : ModelConfigViewModel
    {
        public override Task Load()
        {
            Items.Clear();
            if (configService.Active != null)
            {
                foreach (var model in configService.Active.TranscriptionModels)
                {
                    Items.Add(model);
                }
            }

            return Task.CompletedTask;
        }
    }
}
