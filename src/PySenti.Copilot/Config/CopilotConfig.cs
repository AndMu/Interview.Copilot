namespace PySenti.Copilot.Config
{
    public record CopilotConfig
    {
        public List<AiModel> LlmModels { get; set; } = new();

        public List<AiModel> TranscriptionModels { get; set; } = new();

        public string? ShortLlm { get; set; }

        public string? LongLlm { get; set; }

        public string? Transcription { get; set; }        
        
        public Language? Language { get; set; } = Config.Language.English;
        
        public string? SelectedAudioDevice { get; set; }
        
        public bool IsOutputAudio { get; set; } = true;
        
        public string? SelectedPromptRole { get; set; }
    }
}
