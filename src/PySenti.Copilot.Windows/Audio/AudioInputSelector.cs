using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace PySenti.Copilot.Windows.Audio
{
    public class AudioInputSelector : IAudioSelector
    {
        private readonly ILogger<AudioInputSelector> logger;        
        private List<AudioDevice> inputDevices = new();

        public AudioInputSelector(ILogger<AudioInputSelector> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            this.logger = logger;
        }

        public IEnumerable<AudioDevice> Devices => inputDevices;

        public AudioDevice? Selected { get; set; }

        public void LoadDevices()
        {
            logger.LogDebug("Loading audio devices...");
            inputDevices = new();

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);                
                inputDevices.Add(new AudioDevice
                {
                    Name = capabilities.ProductName,
                    Id = i,
                });
            }

            logger.LogDebug("Loaded {0} devices.", inputDevices.Count);
        }

        public IWaveIn Aquire()
        {
            return new WaveInEvent
            {
                DeviceNumber = Selected.Id,
                WaveFormat = new WaveFormat(44100, 1)
            };
        }
    }
}
