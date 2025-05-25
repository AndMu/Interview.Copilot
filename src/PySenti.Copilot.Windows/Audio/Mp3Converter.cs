using NAudio.Lame;
using NAudio.Wave;

namespace PySenti.Copilot.Windows.Audio
{
    public class Mp3Converter : IMp3Converter
    {
        public async Task Convert(string inputWavFile, string outputMp3File)
        {
            await using (var reader = new WaveFileReader(inputWavFile))
            await using (var fileWriter = new LameMP3FileWriter(outputMp3File, reader.WaveFormat, LAMEPreset.VBR_90))
            {
                await reader.CopyToAsync(fileWriter);
            }
            
            File.Delete(inputWavFile);
        }
    }
}
