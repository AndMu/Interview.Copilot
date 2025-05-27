using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PySenti.Copilot.Config;
using PySenti.Copilot.Prompts;
using System.Windows;

namespace PySenti.Copilot.App.ViewModels.Config
{
    public class ConfigurationViewModel : ObservableObject
    {
        private readonly ICopilotConfigService configService;
        private readonly IInterviewPromptConfigurator interviewPromptConfigurator;

        public ConfigurationViewModel(ICopilotConfigService configService, IInterviewPromptConfigurator interviewPromptConfigurator)
        {
            ArgumentNullException.ThrowIfNull(interviewPromptConfigurator);
            this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
            this.interviewPromptConfigurator = interviewPromptConfigurator;

            LLmViewModel = new LLmConfigViewModel(configService);
            TranscriptionViewModel = new TranscriptionConfigViewModel(configService);
            PromptViewModel = new PromptConfigViewModel(interviewPromptConfigurator);

            SaveCommand = new RelayCommand(SaveConfigAsync);
            CancelCommand = new RelayCommand(Cancel);
        }

        public LLmConfigViewModel LLmViewModel { get; }

        public TranscriptionConfigViewModel TranscriptionViewModel { get; }

        public PromptConfigViewModel PromptViewModel { get; }

        // Commands
        public IRelayCommand SaveCommand { get; }

        public IRelayCommand CancelCommand { get; }

        // Action to be set by the view for closing
        public Action<bool>? CloseAction { get; set; }

        public async Task Load()
        {
            try
            {
                await configService.Load();
                await TranscriptionViewModel.Load();
                await LLmViewModel.Load();
                await PromptViewModel.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveConfigAsync()
        {
            try
            {
                if (configService.Active != null)
                {
                    await configService.Save(configService.Active);
                }

                await interviewPromptConfigurator.Save();
                MessageBox.Show("Configuration saved successfully.", "Save Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}