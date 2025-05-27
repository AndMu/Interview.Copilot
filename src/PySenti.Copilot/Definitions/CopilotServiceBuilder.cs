using Microsoft.Extensions.DependencyInjection;
using PySenti.Copilot.Config;
using PySenti.Copilot.Prompts;

namespace PySenti.Copilot.Definitions
{
    public static class CopilotServiceBuilder
    {
        public static IServiceCollection AddCopilot(this IServiceCollection services)
        {
            services.AddSingleton<ICopilotConfigService, CopilotConfigService>();
            services.AddSingleton<ICopilotService, CopilotService>();
            services.AddSingleton<IInterviewPromptConfigurator, InterviewPromptConfigurator>();
            services.AddSingleton<IInterviewPromptGenerator, InterviewPromptGenerator>();
            return services;
        }
    }
}
