#nullable enable

using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Speech configuration for text-to-speech generation.
    /// </summary>
    public record SpeechConfig
    {
        /// <summary>
        /// Voice configuration for the generated speech.
        /// </summary>
        [JsonPropertyName("voiceConfig")]
        public VoiceConfig? VoiceConfig { get; set; }
    }

    /// <summary>
    /// Voice configuration for text-to-speech generation.
    /// </summary>
    public record VoiceConfig
    {
        /// <summary>
        /// Configuration for using a prebuilt voice.
        /// </summary>
        [JsonPropertyName("prebuiltVoiceConfig")]
        public PrebuiltVoiceConfig? PrebuiltVoiceConfig { get; set; }
    }

    /// <summary>
    /// Configuration for using a prebuilt voice.
    /// </summary>
    public record PrebuiltVoiceConfig
    {
        /// <summary>
        /// The name of the prebuilt voice to use.
        /// Available voices: Aoede, Charon, Fenrir, Kore, Orpheus, Puck
        /// </summary>
        [JsonPropertyName("voiceName")]
        public string VoiceName { get; set; }

        public PrebuiltVoiceConfig(string voiceName)
        {
            VoiceName = voiceName;
        }
    }

    /// <summary>
    /// Available prebuilt voices for text-to-speech.
    /// </summary>
    public static class PrebuiltVoices
    {
        public const string Aoede = "Aoede";
        public const string Charon = "Charon";
        public const string Fenrir = "Fenrir";
        public const string Kore = "Kore";
        public const string Orpheus = "Orpheus";
        public const string Puck = "Puck";
    }
}