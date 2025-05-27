using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Polly;
using Polly.Retry;
using PySenti.Copilot.Config;
using PySenti.Copilot.Definitions;
using System.Diagnostics.CodeAnalysis;

namespace PySenti.Copilot.Logic
{
    public class TranscriptionService : ITranscriptionService
    {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly IAudioToTextService audioToText;

        private readonly AsyncRetryPolicy retryPolicy;

        [Experimental("SKEXP0001")]
        public TranscriptionService(IAudioToTextService audioToText) 
        {
            this.audioToText = audioToText;
            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<string> TranscribeAudioAsync(string audioFilePath, Language language, CancellationToken token)
        {
#pragma warning disable SKEXP0010
            OpenAIAudioToTextExecutionSettings executionSettings = new(audioFilePath)
#pragma warning restore SKEXP0010
            {
                Language = language.ToLanguageCode(),
                Temperature = 0.0f 
            };

            // Read audio content from a file
            await using var audioFileStream = File.OpenRead(audioFilePath);
            var audioFileBinaryData = await BinaryData.FromStreamAsync(audioFileStream!, token);
            AudioContent audioContent = new(audioFileBinaryData, mimeType: null);
            var result = await retryPolicy.ExecuteAsync(async () => await audioToText.GetTextContentAsync(audioContent, executionSettings, cancellationToken: token));
            return result.Text!;
        }

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}