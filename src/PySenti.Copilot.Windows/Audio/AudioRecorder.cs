using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace PySenti.Copilot.Windows.Audio
{
    public class AudioRecorder : IAudioRecorder
    {
        private readonly ILogger<AudioRecorder> logger;
        private readonly IMp3Converter mp3Converter;
        private string outputFilePath;
        private IWaveIn? waveIn;
        private WaveFileWriter? writer;

        public AudioRecorder(ILogger<AudioRecorder> logger, IMp3Converter mp3Converter)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mp3Converter);
            this.logger = logger;
            this.mp3Converter = mp3Converter;
        }

        public bool IsRecording => waveIn != null;

        public string? ResultFile { get; private set; } 

        public void Start(IAudioSelector source)
        {
            logger.LogDebug("Starting audio recording...");
            ArgumentNullException.ThrowIfNull(source);
            ResultFile = null;
            waveIn = source.Aquire();

            outputFilePath = $"Recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);

            waveIn.DataAvailable += DataAvailable;
            waveIn.RecordingStopped += RecordingStopped;

            waveIn.StartRecording();
            
        }
        public async Task<bool> Stop()
        {
            if (waveIn == null) return false;
            logger.LogDebug("Stopping audio recording...");
            
            waveIn.StopRecording();
            waveIn.DataAvailable -= DataAvailable;
            waveIn.RecordingStopped -= RecordingStopped;
            if (writer != null)
            {
                await writer.DisposeAsync();
                writer = null;
            }

            waveIn?.Dispose();
            waveIn = null;

            ResultFile = outputFilePath.Replace(".wav", ".mp3");
            await mp3Converter.Convert(outputFilePath, ResultFile);
            logger.LogDebug("Audio recording saved to {0}", ResultFile);
            return true;
        }

        private void RecordingStopped(object sender, StoppedEventArgs e)
        {
            logger.LogDebug("Audio recording stopped.");
            waveIn?.Dispose();
            waveIn = null;
        }

        private void DataAvailable(object sender, WaveInEventArgs e)
        {
            writer?.Write(e.Buffer, 0, e.BytesRecorded);
        }      
    }
}
