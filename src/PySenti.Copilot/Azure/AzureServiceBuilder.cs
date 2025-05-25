using Microsoft.Extensions.DependencyInjection;
using PySenti.Copilot.Definitions;

namespace PySenti.Copilot.Azure
{
    public static class AzureServiceBuilder
    {
        public static IServiceCollection AddAzure(this IServiceCollection services)
        {
            services.AddTransient<ILlmFactory, AzureFactory>();
            services.AddTransient<ITranscriptionFactory, AzureFactory>();
            return services;
        }
    }
}
