using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PySenti.Copilot.Prompts;

public class InterviewPromptConfigurator : IInterviewPromptConfigurator
{
    private readonly string promptFilePath;

    private readonly JsonSerializerOptions options;

    public InterviewPromptConfigurator()
    {
        promptFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Prompts", "prompt.json");
        options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }

    public AllPrompts? Prompts { get; set; }

    public async Task Load()
    {
        var jsonContent = await File.ReadAllTextAsync(promptFilePath);
        Prompts = JsonSerializer.Deserialize<AllPrompts>(jsonContent, options);
    }

    public async Task Save()
    {
        var jsonContent = JsonSerializer.Serialize(Prompts, options);
        await File.WriteAllTextAsync(promptFilePath, jsonContent);
    }
}