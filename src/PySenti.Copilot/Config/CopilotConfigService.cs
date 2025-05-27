using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PySenti.Copilot.Config
{
    public class CopilotConfigService : ICopilotConfigService
    {
        private readonly ILogger<CopilotConfigService> logger;

        private readonly JsonSerializerOptions options;

        public CopilotConfigService(ILogger<CopilotConfigService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            this.logger = logger;
            Location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config", "config.json");
            options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
        }

        public string Location { get; }

        public CopilotConfig? Active { get; private set; }

        public async Task<CopilotConfig> Load()
        {            
            if (!File.Exists(Location))
            {
                logger.LogWarning("Configuration file not found at {Location}. Creating default configuration.", Location);
                Active = CreateDefaultConfiguration();
                return Active;
            }

            try
            {
                logger.LogDebug("Loading configuration from {Location}", Location);
                var json = await File.ReadAllTextAsync(Location);              

                Active = JsonSerializer.Deserialize<CopilotConfig>(json, options);
                Active ??= CreateDefaultConfiguration();
                return Active;

            }
            catch
            {
                Active = CreateDefaultConfiguration();
                return Active;
            }
        }

        public async Task Save(CopilotConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);
            logger.LogDebug("Saving configuration to {Location}", Location);
            var json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(Location, json);
        }

        private static CopilotConfig CreateDefaultConfiguration()
        {
            var config = new CopilotConfig();

            config.LlmModels.Add(
                new AiModel
                {
                    Model = "gpt-4",
                    Vendor = Vendors.AzureOpenAi,
                    ApiKey = "your-api-key",
                    Endpoint = "https://your-endpoint.openai.azure.com/"
                });

            config.LlmModels.Add(
                new AiModel
                {
                    Model = "gpt-4-32k",
                    Vendor = Vendors.AzureOpenAi,
                    ApiKey = "your-api-key",
                    Endpoint = "https://your-endpoint.openai.azure.com/"
                });

            config.TranscriptionModels.Add(
                new AiModel
                {
                    Model = "whisper-1",
                    Vendor = Vendors.AzureOpenAi,
                    ApiKey = "your-api-key",
                    Endpoint = "https://your-endpoint.openai.azure.com/"
                });

            return config;
        }
    }
}