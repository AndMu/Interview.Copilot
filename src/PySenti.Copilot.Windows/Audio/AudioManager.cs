namespace PySenti.Copilot.Windows.Audio
{
    public class AudioManager : IAudioManager
    {
        public AudioManager(AudioInputSelector input, AudioOutputSelector output, IAudioRecorder recorder)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));
        }

        public IAudioSelector Input { get; }

        public IAudioSelector Output { get; }

        public IAudioRecorder Recorder { get; }
    }
}
