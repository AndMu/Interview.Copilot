using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.Logging;
using PySenti.Copilot.App.Helpers;
using PySenti.Copilot.App.ViewModels;

namespace PySenti.Copilot.App.Views
{
    public partial class MainWindow : Window
    {
        private bool hasClosed = false;

        private readonly ILogger<MainWindow> logger;

        private readonly IStatePersistence state;

        private readonly MainWindowViewModel viewModel;

        public MainWindow(ILogger<MainWindow> logger, MainWindowViewModel viewModel, IStatePersistence state)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(viewModel);
            ArgumentNullException.ThrowIfNull(state);
            this.logger = logger;
            this.viewModel = viewModel;
            this.state = state;
            DataContext = viewModel;
            InitializeComponent();
            Closing += MainWindow_Closing;
            Loaded += MainWindow_Loaded;
        }
       
        public MainWindow(ILogger<MainWindow> logger, IStatePersistence state, MainWindowViewModel viewModel)
        {
            this.logger = logger;
            this.state = state;
            this.viewModel = viewModel;
            InitializeComponent();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during closing tasks.");
            }
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            try
            {
                if (hasClosed)
                {
                    return;
                }

                logger.LogInformation("MainWindow is closing. Executing closing tasks...");
                e.Cancel = true;
                await ClosingTasks();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during closing tasks.");
            }
        }

        private async Task ClosingTasks()
        {
            await state.Save(viewModel);
            hasClosed = true;
            Close();
        }
    }
}