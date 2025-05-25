using Microsoft.Extensions.DependencyInjection;

namespace PySenti.Copilot.Windows.Audio
{
    public static class AudioServiceBuilder
    {
        public static IServiceCollection AddAudio(this IServiceCollection services)
        {
            services.AddSingleton<IMp3Converter, Mp3Converter>();
            services.AddSingleton<AudioInputSelector>();
            services.AddSingleton<AudioOutputSelector>();
            services.AddTransient<IAudioRecorder, AudioRecorder>();
            services.AddTransient<IAudioManager, AudioManager>();            
            return services;
        }
    }
}
