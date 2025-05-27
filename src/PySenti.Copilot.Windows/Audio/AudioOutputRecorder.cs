using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace PySenti.Copilot.Windows.Audio
{
    public class AudioOutputSelector : IAudioSelector
    {
        private readonly ILogger<AudioOutputSelector> logger;
        private readonly Dictionary<AudioDevice, MMDevice> outputDevices = new();

        public AudioOutputSelector(ILogger<AudioOutputSelector> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            this.logger = logger;
        }

        public IEnumerable<AudioDevice> Devices => outputDevices.Select(item => item.Key);

        public AudioDevice Selected { get; set; }

        public void LoadDevices()
        {
            logger.LogDebug("Loading audio devices...");
            using var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            for (var i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                var audioDevice = new AudioDevice
                {
                    Id = i,
                    Name = device.FriendlyName
                };

                outputDevices[audioDevice] = device;
            }

            logger.LogDebug("Loaded {0} devices.", outputDevices.Count);
        }

        public IWaveIn Aquire()
        {
            var selectedDevice = outputDevices.GetValueOrDefault(Selected);
            if (selectedDevice == null)
            {
                throw new InvalidOperationException("No selected device.");
            }

            return new WasapiLoopbackCapture(selectedDevice);
        }
    }
}
