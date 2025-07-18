#nullable enable

using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Speech configuration for text-to-speech generation.
    /// Used primarily with the Gemini Live API for audio generation.
    /// 
    /// https://ai.google.dev/api/generate-content#SpeechConfig
    /// </summary>
    public record SpeechConfig
    {
        /// <summary>
        /// The configuration in case of single-voice output.
        /// </summary>
        [JsonPropertyName("voiceConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public VoiceConfig? VoiceConfig { get; set; }

        /// <summary>
        /// Optional. The configuration for the multi-speaker setup.
        /// It is mutually exclusive with the voiceConfig field.
        /// </summary>
        [JsonPropertyName("multiSpeakerVoiceConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MultiSpeakerVoiceConfig? MultiSpeakerVoiceConfig { get; set; }

        /// <summary>
        /// Optional. Language code (in BCP 47 format, e.g. "en-US") for speech synthesis.
        /// 
        /// Valid values are: de-DE, en-AU, en-GB, en-IN, en-US, es-US, fr-FR, hi-IN, pt-BR, ar-XA, es-ES, fr-CA, id-ID, it-IT, ja-JP, tr-TR, vi-VN, bn-IN, gu-IN, kn-IN, ml-IN, mr-IN, ta-IN, te-IN, nl-NL, ko-KR, cmn-CN, pl-PL, ru-RU, and th-TH.
        /// </summary>
        [JsonPropertyName("languageCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LanguageCode { get; set; }


        [JsonConstructor]
        public SpeechConfig(VoiceConfig? voiceConfig = null, MultiSpeakerVoiceConfig? multiSpeakerVoiceConfig = null, string? languageCode = null)
        {
            VoiceConfig = voiceConfig;
            MultiSpeakerVoiceConfig = multiSpeakerVoiceConfig;
            LanguageCode = languageCode;
        }

        // Constructor syntax sugars
        public SpeechConfig(Voice voice, LanguageCode? languageCode = null)
        {
            VoiceConfig = voice;
            LanguageCode = languageCode?.ToCodeString() ?? null;
        }
        public SpeechConfig(SpeakerVoiceConfig[] speakers, LanguageCode? languageCode = null)
        {
            MultiSpeakerVoiceConfig = speakers;
            LanguageCode = languageCode?.ToCodeString() ?? null;
        }
    }

    /// <summary>
    /// Voice configuration for text-to-speech generation.
    /// https://ai.google.dev/api/generate-content#VoiceConfig
    /// </summary>
    public record VoiceConfig
    {
        /// <summary>
        /// Optional. Configuration for using a prebuilt voice.
        /// </summary>
        [JsonPropertyName("prebuiltVoiceConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PrebuiltVoiceConfig? PrebuiltVoiceConfig { get; set; }

        // Constructor syntax sugars
        public static implicit operator VoiceConfig(Voice voice) => new() { PrebuiltVoiceConfig = new PrebuiltVoiceConfig(voice) };
    }

    /// <summary>
    /// The configuration for the prebuilt speaker to use.
    /// https://ai.google.dev/api/generate-content#PrebuiltVoiceConfig
    /// </summary>
    public record PrebuiltVoiceConfig
    {
        /// <summary>
        /// The name of the preset voice to use.
        /// </summary>
        [JsonPropertyName("voiceName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string VoiceName { get; set; }

        [JsonConstructor]
        public PrebuiltVoiceConfig(string voiceName) => VoiceName = voiceName;
        public PrebuiltVoiceConfig(Voice voice) : this(voice.ToString()) { }
    }

    /// <summary>
    /// Configuration for multi-speaker voice setup.
    /// https://ai.google.dev/api/generate-content#MultiSpeakerVoiceConfig
    /// </summary>
    public record MultiSpeakerVoiceConfig
    {
        /// <summary>
        /// Required. All the enabled speaker voices.
        /// </summary>
        [JsonPropertyName("speakerVoiceConfigs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SpeakerVoiceConfig[]? SpeakerVoiceConfigs { get; set; }

        // Constructor syntax sugars
        public static implicit operator MultiSpeakerVoiceConfig(SpeakerVoiceConfig[] speakers) => new() { SpeakerVoiceConfigs = speakers };
    }

    /// <summary>
    /// Configuration for a single speaker in a multi-speaker setup.
    /// </summary>
    public record SpeakerVoiceConfig
    {
        /// <summary>
        /// Required. The name of the speaker to use. Should be the same as in the prompt.
        /// </summary>
        [JsonPropertyName("speaker")]
        public string Speaker { get; set; }

        /// <summary>
        /// Required. The configuration for the voice to use.
        /// </summary>
        [JsonPropertyName("voiceConfig")]
        public VoiceConfig VoiceConfig { get; set; }

        public SpeakerVoiceConfig(string speaker, VoiceConfig voiceConfig)
        {
            Speaker = speaker;
            VoiceConfig = voiceConfig;
        }
    }

    /// <summary>
    /// TTS models support the following 30 voice options in the voice_name field:
    /// 
    /// https://ai.google.dev/gemini-api/docs/speech-generation#voices
    /// You can hear all the voice options in the  AI Studio:
    /// https://aistudio.google.com/generate-speech
    /// </summary>
    public enum Voice
    {
        /// <summary>Bright</summary>
        Zephyr,
        /// <summary>Upbeat</summary>
        Puck,
        /// <summary>Informative</summary>
        Charon,
        /// <summary>Firm</summary>
        Kore,
        /// <summary>Excitable</summary>
        Fenrir,
        /// <summary>Youthful</summary>
        Leda,
        /// <summary>Firm</summary>
        Orus,
        /// <summary>Breezy</summary>
        Aoede,
        /// <summary>Easy-going</summary>
        Callirrhoe,
        /// <summary>Bright</summary>
        Autonoe,
        /// <summary>Breathy</summary>
        Enceladus,
        /// <summary>Clear</summary>
        Iapetus,
        /// <summary>Easy-going</summary>
        Umbriel,
        /// <summary>Smooth</summary>
        Algieba,
        /// <summary>Smooth</summary>
        Despina,
        /// <summary>Clear</summary>
        Erinome,
        /// <summary>Gravelly</summary>
        Algenib,
        /// <summary>Informative</summary>
        Rasalgethi,
        /// <summary>Upbeat</summary>
        Laomedeia,
        /// <summary>Soft</summary>
        Achernar,
        /// <summary>Firm</summary>
        Alnilam,
        /// <summary>Even</summary>
        Schedar,
        /// <summary>Mature</summary>
        Gacrux,
        /// <summary>Forward</summary>
        Pulcherrima,
        /// <summary>Friendly</summary>
        Achird,
        /// <summary>Casual</summary>
        Zubenelgenubi,
        /// <summary>Gentle</summary>
        Vindemiatrix,
        /// <summary>Lively</summary>
        Sadachbia,
        /// <summary>Knowledgeable</summary>
        Sadaltager,
        /// <summary>Warm</summary>
        Sulafat
    }

    /// <summary>
    /// The TTS models detect the input language automatically. They support the following 24 languages:
    /// 
    /// https://ai.google.dev/gemini-api/docs/speech-generation#languages
    /// </summary>
    public enum LanguageCode
    {
        /// <summary>Arabic (Egyptian)</summary>
        ar_EG,
        /// <summary>Bengali (Bangladesh)</summary>
        bn_BD,
        /// <summary>German (Germany)</summary>
        de_DE,
        /// <summary>English (India)</summary>
        en_IN,
        /// <summary>English (US)</summary>
        en_US,
        /// <summary>Spanish (US)</summary>
        es_US,
        /// <summary>French (France)</summary>
        fr_FR,
        /// <summary>Hindi (India)</summary>
        hi_IN,
        /// <summary>Indonesian (Indonesia)</summary>
        id_ID,
        /// <summary>Italian (Italy)</summary>
        it_IT,
        /// <summary>Japanese (Japan)</summary>
        ja_JP,
        /// <summary>Korean (Korea)</summary>
        ko_KR,
        /// <summary>Marathi (India)</summary>
        mr_IN,
        /// <summary>Dutch (Netherlands)</summary>
        nl_NL,
        /// <summary>Polish (Poland)</summary>
        pl_PL,
        /// <summary>Portuguese (Brazil)</summary>
        pt_BR,
        /// <summary>Romanian (Romania)</summary>
        ro_RO,
        /// <summary>Russian (Russia)</summary>
        ru_RU,
        /// <summary>Tamil (India)</summary>
        ta_IN,
        /// <summary>Telugu (India)</summary>
        te_IN,
        /// <summary>Thai (Thailand)</summary>
        th_TH,
        /// <summary>Turkish (Turkey)</summary>
        tr_TR,
        /// <summary>Ukrainian (Ukraine)</summary>
        uk_UA,
        /// <summary>Vietnamese (Vietnam)</summary>
        vi_VN
    }

    /// <summary>
    /// Extension methods for LanguageCode enum.
    /// </summary>
    public static class LanguageCodeExtensions
    {
        /// <summary>
        /// Converts the LanguageCode enum to its BCP 47 string representation.
        /// </summary>
        public static string ToCodeString(this LanguageCode languageCode)
        {
            return languageCode.ToString().Replace('_', '-');
        }
    }
}
