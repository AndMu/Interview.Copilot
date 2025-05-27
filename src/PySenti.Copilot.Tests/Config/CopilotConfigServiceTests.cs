using Microsoft.Extensions.Logging.Abstractions;
using PySenti.Copilot.Config;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PySenti.Copilot.Tests.Config
{
    [TestFixture]
    public class CopilotConfigServiceTests
    {
        private CopilotConfigService configService;

        [SetUp]
        public void Setup()
        {
            configService = new CopilotConfigService(new NullLogger<CopilotConfigService>());           
        }     

      
        [Test]
        public async Task LoadWithCustom()
        {
            var testConfig = new CopilotConfig();
            testConfig.LlmModels.Add(new AiModel
            {
                Model = "test-model",
                Vendor = Vendors.AzureOpenAi,
                ApiKey = "test-key",
                Endpoint = "https://test-endpoint.com"
            });

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var json = JsonSerializer.Serialize(testConfig, options);
            await File.WriteAllTextAsync(configService.Location, json);

            // Act
            var loadedConfig = await configService.Load();

            // Assert
            Assert.That(loadedConfig, Is.Not.Null);
            Assert.That(loadedConfig.LlmModels, Has.Count.EqualTo(1));
            Assert.That(loadedConfig.LlmModels[0].Model, Is.EqualTo("test-model"));
            Assert.That(loadedConfig.LlmModels[0].ApiKey, Is.EqualTo("test-key"));
            Assert.That(loadedConfig.LlmModels[0].Endpoint, Is.EqualTo("https://test-endpoint.com"));
        }

        [Test]
        public async Task LoadWithInvalid()
        {
            // Arrange - create an invalid JSON file
            await File.WriteAllTextAsync(configService.Location, "{ this is not valid JSON }");

            // Act
            var config = await configService.Load();

            // Assert
            Assert.That(config, Is.Not.Null);
            Assert.That(config.LlmModels, Has.Count.EqualTo(2));
            Assert.That(config.TranscriptionModels, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task Save()
        {
            // Arrange
            var testConfig = new CopilotConfig();
            testConfig.LlmModels.Add(new AiModel
            {
                Model = "test-save-model",
                Vendor = Vendors.AzureOpenAi,
                ApiKey = "test-save-key",
                Endpoint = "https://test-save-endpoint.com"
            });

            await configService.Save(testConfig);

            // Assert
            Assert.That(File.Exists(configService.Location), Is.True);
            
            // Read the file and check if it contains our data
            var json = await File.ReadAllTextAsync(configService.Location);
            Assert.That(json, Does.Contain("test-save-model"));
            Assert.That(json, Does.Contain("test-save-key"));
            Assert.That(json, Does.Contain("https://test-save-endpoint.com"));

            var loaded = await configService.Load();
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.LlmModels, Has.Count.EqualTo(1));
            Assert.That(loaded.LlmModels[0].Model, Is.EqualTo("test-save-model"));
        }
    }
}
