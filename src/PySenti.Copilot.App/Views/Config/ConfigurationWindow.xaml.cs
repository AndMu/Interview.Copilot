using PySenti.Copilot.App.ViewModels.Config;
using System.Windows;

namespace PySenti.Copilot.App.Views.Config
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow(ConfigurationViewModel model)
        {
            InitializeComponent();
            DataContext = model;
            // Set the CloseAction to close this window
            model.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
