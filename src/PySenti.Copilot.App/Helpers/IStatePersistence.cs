using PySenti.Copilot.App.ViewModels;

namespace PySenti.Copilot.App.Helpers
{
    public interface IStatePersistence
    {
        Task<bool> Save(MainWindowViewModel viewModel);
    }
}