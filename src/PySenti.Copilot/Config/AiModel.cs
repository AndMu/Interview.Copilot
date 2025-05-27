namespace PySenti.Copilot.Config
{
    public class AiModel // it can't be record - bindings will not work properly
    {
        public string Model { get; init; } = null!;

        public Vendors Vendor { get; init; } = Vendors.AzureOpenAi;

        public string ApiKey { get; init; } = null!;

        public string Endpoint { get; init; } = null!;
    }
}
