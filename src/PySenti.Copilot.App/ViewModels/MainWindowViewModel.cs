using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PySenti.Copilot.App.Helpers;
using PySenti.Copilot.App.ViewModels.Config;
using PySenti.Copilot.App.Views.Config;
using PySenti.Copilot.Config;
using PySenti.Copilot.Definitions;
using PySenti.Copilot.Prompts;
using PySenti.Copilot.Windows.Audio;
using PySenti.Copilot.Windows.Screens;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace PySenti.Copilot.App.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        // Services
        private readonly IAudioManager audio;
        private readonly IScreenShot screenShot;
        private readonly ICopilotService copilotService;
        private readonly IImageHelper imageHelper;
        private readonly IInterviewPromptConfigurator interviewPromptConfigurator;
        private readonly IInterviewPromptGenerator promptGenerator;
        private ITranscriptionService? transcriptionService;
        private ILlmService? shortLlmService;
        private ILlmService? longLlmService;

        // Private fields
        private bool isRecording;
        private string statusText = "Ready";
        private bool hasError;
        private string transcription = "";
        private string shortAnswer = "";
        private string detailedAnswer = "";
        private string timerDisplay = "00:00:00";
        private DateTime recordingStartTime;
        private DispatcherTimer? timer;
        private AudioDevice? selectedAudioDevice;
        private bool isInputSource = true;
        private Language selectedLanguage = Language.English; // Default to English
        private BitmapImage? screenshotImage;
        private bool isProcessing;
        private Screen? selectedMonitor;
        private string? selectedShortLlm;
        private string? selectedLongLlm;
        private string? selectedTranscription;
        private Bitmap? currentScreenshot;
        private InterviewPromptDefinition? selectedPrompt;

        // Initialization fields
        private bool isInitialized;
        private bool isInitializing;
        private string initializationStatus = "Not initialized";

        public MainWindowViewModel(
            ICopilotService copilotService,
            IAudioManager audio,
            IScreenShot screenShot,
            IImageHelper imageHelper,
            IInterviewPromptConfigurator interviewPromptConfigurator,
            IInterviewPromptGenerator promptGenerator)
        {
            ArgumentNullException.ThrowIfNull(copilotService);
            ArgumentNullException.ThrowIfNull(audio);
            ArgumentNullException.ThrowIfNull(screenShot);
            ArgumentNullException.ThrowIfNull(imageHelper);
            ArgumentNullException.ThrowIfNull(promptGenerator);

            this.screenShot = screenShot;
            this.imageHelper = imageHelper;
            this.copilotService = copilotService;
            this.audio = audio;
            this.interviewPromptConfigurator = interviewPromptConfigurator;
            this.promptGenerator = promptGenerator;

            // Create commands
            InitializeCommand = new AsyncRelayCommand(InitializeAsync, () => !IsInitializing);
            RecordCommand = new AsyncRelayCommand(ToggleRecording, () => !IsProcessing && IsInitialized);
            TakeScreenshotCommand = new RelayCommand(TakeScreenshot, () => IsInitialized && !IsProcessing);
            DeleteScreenshotCommand = new RelayCommand(DeleteScreenshot, () => ScreenshotImage != null && IsInitialized && !IsProcessing);
            LoadDevicesCommand = new RelayCommand(LoadAudioDevices, () => IsInitialized);
            OpenConfigurationCommand = new AsyncRelayCommand(OpenConfiguration);
           
            // Setup timer
            SetupTimer();
        }

        public IAsyncRelayCommand InitializeCommand { get; }

        public IRelayCommand RecordCommand { get; }

        public IRelayCommand TakeScreenshotCommand { get; }

        public IRelayCommand DeleteScreenshotCommand { get; }

        public IRelayCommand LoadDevicesCommand { get; }

        public IRelayCommand OpenConfigurationCommand { get; }

        public IEnumerable<AudioDevice> AudioDevices => IsInputSource ? audio.Input.Devices : audio.Output.Devices;

        public Screen?[] Monitors => screenShot.Screens;

        public string[] AvailableLlms => copilotService.Llms;

        public string[] AvailableTranscriptions => copilotService.Transcriptions;

        public Language[] AvailableLanguages => Enum.GetValues<Language>();

        public IEnumerable<InterviewPromptDefinition> AvailablePrompts => interviewPromptConfigurator.Prompts?.Definitions ?? Enumerable.Empty<InterviewPromptDefinition>();

        public bool IsInitialized
        {
            get => isInitialized;
            private set => SetProperty(ref isInitialized, value);
        }

        public bool IsInitializing
        {
            get => isInitializing;
            private set
            {
                SetProperty(ref isInitializing, value);
                InitializeCommand.NotifyCanExecuteChanged();
            }
        }

        public string InitializationStatus
        {
            get => initializationStatus;
            private set => SetProperty(ref initializationStatus, value);
        }

        public bool IsRecording
        {
            get => isRecording;
            set
            {
                SetProperty(ref isRecording, value);
                OnPropertyChanged(nameof(RecordButtonText));
                UpdateCommandStates();
            }
        }

        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                SetProperty(ref isProcessing, value);
                UpdateCommandStates();
            }
        }

        public bool HasError
        {
            get => hasError;
            private set => SetProperty(ref hasError, value);
        }

        public string StatusText
        {
            get => statusText;
            set
            {
                // Check if the status text indicates an error
                HasError = value.StartsWith("Error") || value.Contains("failed");
                SetProperty(ref statusText, value);
            }
        }

        public string Transcription
        {
            get => transcription;
            set => SetProperty(ref transcription, value);
        }

        public string ShortAnswer
        {
            get => shortAnswer;
            set => SetProperty(ref shortAnswer, value);
        }

        public string DetailedAnswer
        {
            get => detailedAnswer;
            set => SetProperty(ref detailedAnswer, value);
        }

        public string TimerDisplay
        {
            get => timerDisplay;
            set => SetProperty(ref timerDisplay, value);
        }

        public AudioDevice? SelectedAudioDevice
        {
            get => selectedAudioDevice;
            set => SetProperty(ref selectedAudioDevice, value);
        }

        public bool IsInputSource
        {
            get => isInputSource;
            set
            {
                if (SetProperty(ref isInputSource, value))
                {
                    // Only load devices if we're initialized
                    if (IsInitialized)
                    {
                        LoadAudioDevices();
                    }

                    OnPropertyChanged(nameof(AudioDevices));
                    UpdateCommandStates();
                }
            }
        }

        public bool IsOutputSource
        {
            get => !isInputSource;
            set
            {
                var newValue = !value;
                if (isInputSource != newValue)
                {
                    isInputSource = newValue;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsInputSource));
                    OnPropertyChanged(nameof(AudioDevices));

                    // Only load devices if we're initialized
                    if (IsInitialized)
                    {
                        LoadAudioDevices();
                    }

                    UpdateCommandStates();
                }
            }
        }

        public Language SelectedLanguage
        {
            get => selectedLanguage;
            set => SetProperty(ref selectedLanguage, value);
        }

        public Screen? SelectedMonitor
        {
            get => selectedMonitor;
            set
            {
                if (Equals(selectedMonitor, value))
                {
                    return;
                }

                selectedMonitor = value;
                screenShot.Selected = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage? ScreenshotImage
        {
            get => screenshotImage;
            set
            {
                SetProperty(ref screenshotImage, value);
                OnPropertyChanged(nameof(HasScreenshot));
                DeleteScreenshotCommand.NotifyCanExecuteChanged();
            }
        }

        public bool HasScreenshot => ScreenshotImage != null;

        private void SetLlm(ref string? selectedLlm, string value, Action<ILlmService?> setService, string errorContext)
        {
            if (value == selectedLlm || string.IsNullOrEmpty(value))
            {
                return;
            }
            try
            {
                if (IsInitialized || IsInitializing)
                {
                    setService(copilotService.Resolve(value));
                }

                selectedLlm = value;
                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                StatusText = $"Error setting {errorContext} LLM: {ex.Message}";
            }
        }

        public string SelectedShortLlm
        {
            get => selectedShortLlm ?? string.Empty;
            set => SetLlm(ref selectedShortLlm, value, s => shortLlmService = s, "Short");
        }

        public string SelectedLongLlm
        {
            get => selectedLongLlm ?? string.Empty;
            set => SetLlm(ref selectedLongLlm, value, s => longLlmService = s, "Long");
        }

        public string SelectedTranscription
        {
            get => selectedTranscription ?? string.Empty;
            set
            {
                if (Equals(selectedTranscription, value) || string.IsNullOrEmpty(value))
                {
                    return;
                }

                try
                {
                    if (IsInitialized ||
                        IsInitializing)
                    {
                        transcriptionService = copilotService.ResolveTranscription(value);
                    }

                    selectedTranscription = value;
                    OnPropertyChanged();
                }
                catch (Exception ex)
                {
                    StatusText = $"Error setting transcription service: {ex.Message}";
                }
            }
        }

        public InterviewPromptDefinition? SelectedPrompt
        {
            get => selectedPrompt;
            set => SetProperty(ref selectedPrompt, value);
        }

        public string RecordButtonText => IsRecording ? "Stop" : "Record";

        public bool CanRecord => !IsProcessing && IsInitialized;

        public async Task InitializeAsync()
        {
            if (IsInitializing)
            {
                return;
            }

            // Reset state if already initialized
            if (IsInitialized)
            {
                IsInitialized = false;
            }

            IsInitializing = true;
            InitializationStatus = "Initializing Copilot...";
            StatusText = "Initializing...";
            HasError = false;

            try
            {
                // Refresh copilot configuration
                await copilotService.Refresh();

                // Load monitors
                OnPropertyChanged(nameof(Monitors));
                if (Monitors.Length > 0)
                {
                    SelectedMonitor = Monitors[0];
                }

                // Load audio devices
                LoadAudioDevices();
                

                // Load LLM and transcription services
                OnPropertyChanged(nameof(AvailableLlms));
                OnPropertyChanged(nameof(AvailableTranscriptions));
                OnPropertyChanged(nameof(AvailableLanguages));

                // Load prompts
                await interviewPromptConfigurator.Load();
                OnPropertyChanged(nameof(AvailablePrompts));

                // Set defaults from config if available
                var config = copilotService.ConfigService.Active;
                if (config != null)
                {
                    IsOutputSource = config.IsOutputAudio;
                    if (config.SelectedAudioDevice != null)
                    {
                        var device = AudioDevices.FirstOrDefault(d => d.Name == config.SelectedAudioDevice);
                        if (device != null)
                        {
                            SelectedAudioDevice = device;
                        }
                    }

                    if (config.SelectedPromptRole != null)
                    {
                        var prompt = AvailablePrompts.FirstOrDefault(p => p.Role == config.SelectedPromptRole);
                        if (prompt != null)
                        {
                            SelectedPrompt = prompt;
                        }
                    }

                    if (!string.IsNullOrEmpty(config.ShortLlm) && AvailableLlms.Contains(config.ShortLlm))
                    {
                        SelectedShortLlm = config.ShortLlm;
                    }
                    else if (AvailableLlms.Length > 0)
                    {
                        SelectedShortLlm = AvailableLlms[0];
                    }

                    if (!string.IsNullOrEmpty(config.LongLlm) && AvailableLlms.Contains(config.LongLlm))
                    {
                        SelectedLongLlm = config.LongLlm;
                    }
                    else if (AvailableLlms.Length > 0)
                    {
                        SelectedLongLlm = AvailableLlms[0];
                    }

                    if (!string.IsNullOrEmpty(config.Transcription) && AvailableTranscriptions.Contains(config.Transcription))
                    {
                        SelectedTranscription = config.Transcription;
                    }
                    else if (AvailableTranscriptions.Length > 0)
                    {
                        SelectedTranscription = AvailableTranscriptions[0];
                    }


                    SelectedLanguage = config.Language ?? Language.English;
                }
                else
                {
                    // Fallbacks if config is missing
                    if (AvailableLlms.Length > 0)
                    {
                        SelectedShortLlm = AvailableLlms[0];
                        SelectedLongLlm = AvailableLlms[0];
                    }

                    if (AvailableTranscriptions.Length > 0)
                    {
                        SelectedTranscription = AvailableTranscriptions[0];
                    }

                    SelectedLanguage = Language.English;
                }

                OnPropertyChanged(nameof(SelectedTranscription));
                OnPropertyChanged(nameof(SelectedShortLlm));
                OnPropertyChanged(nameof(SelectedLongLlm));
                OnPropertyChanged(nameof(SelectedLanguage));

                // Set default prompt from AllPrompts
                var prompts = interviewPromptConfigurator.Prompts;
                if (prompts != null && !string.IsNullOrEmpty(prompts.DefaultPrompt))
                {
                    var def = AvailablePrompts.FirstOrDefault(p => p.Role == prompts.DefaultPrompt);
                    if (def != null)
                    {
                        SelectedPrompt = def;
                    }
                    else if (AvailablePrompts.Any())
                    {
                        SelectedPrompt = AvailablePrompts.First();
                    }
                }
                else if (AvailablePrompts.Any())
                {
                    SelectedPrompt = AvailablePrompts.First();
                }

                IsInitialized = true;
                InitializationStatus = "Initialization complete";
                StatusText = "Ready";
            }
            catch (Exception ex)
            {
                InitializationStatus = $"Initialization failed: {ex.Message}";
                StatusText = $"Error: {ex.Message}";
                HasError = true;
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializing = false;
                UpdateCommandStates();
            }
        }

        private void UpdateCommandStates()
        {
            RecordCommand.NotifyCanExecuteChanged();
            TakeScreenshotCommand.NotifyCanExecuteChanged();
            DeleteScreenshotCommand.NotifyCanExecuteChanged();
            LoadDevicesCommand.NotifyCanExecuteChanged();
            OpenConfigurationCommand.NotifyCanExecuteChanged();           
            OnPropertyChanged(nameof(CanRecord));
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += (s, e) =>
            {
                if (IsRecording)
                {
                    TimeSpan elapsed = DateTime.Now - recordingStartTime;
                    TimerDisplay = elapsed.ToString(@"hh\:mm\:ss");
                }
            };
        }

        private void LoadAudioDevices()
        {
            try
            {
                audio.Output.LoadDevices();
                audio.Input.LoadDevices();

                OnPropertyChanged(nameof(AudioDevices));

                if (AudioDevices.Any())
                {
                    SelectedAudioDevice = AudioDevices.First();
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error loading audio devices: {ex.Message}";
            }
        }

        private async Task ToggleRecording()
        {
            if (!IsRecording)
            {
                StartRecording();
            }
            else
            {
                await StopRecording();
            }
        }

        private void StartRecording()
        {
            if (SelectedAudioDevice == null)
            {
                return;
            }

            try
            {
                var selector = IsInputSource ? audio.Input : audio.Output;
                selector.Selected = SelectedAudioDevice;

                audio.Recorder.Start(selector);
                IsRecording = true;
                HasError = false;

                StatusText = "Recording...";
                recordingStartTime = DateTime.Now;
                timer?.Start();

                // Clear previous results
                Transcription = "";
                ShortAnswer = "";
                DetailedAnswer = "";
            }
            catch (Exception ex)
            {
                StatusText = $"Error starting recording: {ex.Message}";
                CleanupRecording();
            }
        }

        private async Task StopRecording()
        {
            if (!IsRecording)
            {
                return;
            }

            IsProcessing = true;
            try
            {
                timer?.Stop();
                await audio.Recorder.Stop();
                string? audioFilePath = audio.Recorder.ResultFile;
                if (transcriptionService == null)
                {
                    throw new InvalidOperationException("Transcription service is not initialized.");
                }

                if (!string.IsNullOrEmpty(audioFilePath))
                {
                    StatusText = "Transcribing...";
                    Transcription = await transcriptionService.TranscribeAudioAsync(audioFilePath, SelectedLanguage, CancellationToken.None);
                    await ProcessLlmResponses(Transcription);
                    StatusText = "Ready";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
            finally
            {
                CleanupRecording();
                IsProcessing = false;
            }
        }

        private async Task ProcessLlmResponses(string transcriptionText)
        {
            try
            {
                byte[]? imageData = null;
                if (currentScreenshot != null)
                {
                    imageData = imageHelper.Serialise(currentScreenshot);
                }

                StatusText = "Getting short answer...";
                ShortAnswer = "";
                DetailedAnswer = "";

                if (SelectedPrompt == null)
                {
                    throw new InvalidOperationException("No prompt selected.");
                }

                var prompt =  promptGenerator.GeneratePrompt(SelectedPrompt);

                var shortAnswerRequest = new LlmRequest
                {
                    System = prompt.Short,
                    Question = transcriptionText,
                    ImageData = imageData
                };

                CancellationToken cancellationToken = CancellationToken.None;
                if (shortLlmService != null)
                {
                    await foreach (var chunk in shortLlmService.GetAnswer(shortAnswerRequest, cancellationToken))
                    {
                        ShortAnswer += chunk;
                    }
                }

                StatusText = "Getting detailed analysis...";

                var detailedAnswerRequest = new LlmRequest
                {
                    System = prompt.Long,
                    Question = transcriptionText,
                    ImageData = imageData
                };

                if (longLlmService != null)
                {
                    await foreach (var chunk in longLlmService.GetAnswer(detailedAnswerRequest, cancellationToken))
                    {
                        DetailedAnswer += chunk;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"LLM processing failed: {ex.Message}", ex);
            }
        }

        private void TakeScreenshot()
        {
            try
            {
                DeleteScreenshot();
                currentScreenshot = screenShot.GetScreenshot();
                if (currentScreenshot != null)
                {
                    ScreenshotImage = imageHelper.Convert(currentScreenshot);
                    StatusText = "Screenshot captured";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Error taking screenshot: {ex.Message}";
            }
        }

        private void DeleteScreenshot()
        {
            if (currentScreenshot != null)
            {
                currentScreenshot.Dispose();
                currentScreenshot = null;
            }

            ScreenshotImage = null;

            if (HasScreenshot)
            {
                StatusText = "Screenshot deleted";
            }
        }

        private void CleanupRecording()
        {
            IsRecording = false;
            timer?.Stop();
            TimerDisplay = "00:00:00";
        }

        private async Task OpenConfiguration()
        {
            var model = new ConfigurationViewModel(copilotService.ConfigService, interviewPromptConfigurator);
            await model.Load();
            var configWindow = new ConfigurationWindow(model);
            var result = configWindow.ShowDialog();

            // If changes were saved, refresh the configuration
            if (result == true)
            {
                StatusText = "Configuration updated. Reinitializing...";
                await InitializeCommand.ExecuteAsync(null);
            }
        }
    }
}