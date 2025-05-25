using PySenti.Copilot.Config;

namespace PySenti.Copilot.App.ViewModels.Config
{
    public abstract class ModelConfigViewModel : BaseConfigViewModel<AiModel>
    {
        protected ModelConfigViewModel()
        {
            AvailableVendors = Enum.GetValues<Vendors>();
        }

        public Vendors[] AvailableVendors { get; set; }

        protected override AiModel Create()
        {
            return new AiModel
            {
                Model = "new-model",
                Vendor = Vendors.AzureOpenAi,
                ApiKey = "your-api-key",
                Endpoint = "https://your-endpoint.openai.azure.com/"
            };
        }

        protected override bool CanSave()
        {
            if (EditingItem == null)
            {
                return false;
            }

            var model = EditingItem;
            return !string.IsNullOrWhiteSpace(model.Model) &&
                   !string.IsNullOrWhiteSpace(model.ApiKey) &&
                   !string.IsNullOrWhiteSpace(model.Endpoint);
        }

    }
}