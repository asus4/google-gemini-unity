#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoogleApis.TTS
{
    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/SynthesisInput
    /// </summary>
    public record TextSynthesizeRequest
    {
        public SynthesisInput input;
        public VoiceSelectionParams voice;
        public AudioConfig audioConfig;

        [JsonConverter(typeof(StringEnumConverter))]
        public TimepointType? enableTimePointing;

        [JsonConstructor]
        public TextSynthesizeRequest(
            SynthesisInput input,
            VoiceSelectionParams voice,
            AudioConfig audioConfig,
            TimepointType? enableTimePointing = null)
        {
            this.input = input;
            this.voice = voice;
            this.audioConfig = audioConfig;
            this.enableTimePointing = enableTimePointing;
        }


    }

    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/text/synthesize
    /// </summary>
    public record TextSynthesizeResponse
    {
        public string audioContent;
        public Timepoint[]? timepoints;
        public AudioConfig audioConfig;

        public TextSynthesizeResponse(string audioContent, Timepoint[]? timepoints, AudioConfig audioConfig)
        {
            this.audioContent = audioContent;
            this.timepoints = timepoints;
            this.audioConfig = audioConfig;
        }
    }

    public record Timepoint
    {
        public string markName;
        public float timeSeconds;

        public Timepoint(string markName, float timeSeconds)
        {
            this.markName = markName;
            this.timeSeconds = timeSeconds;
        }
    }

    public enum TimepointType
    {
        TIMEPOINT_TYPE_UNSPECIFIED, // Not specified. No timepoint information will be returned.
        SSML_MARK,// Timepoint information of <mark> tags in SSML input will be returned.
    }

    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/SynthesisInput
    /// </summary>
    public record SynthesisInput
    {
        // Union field input_source can be only one of the following:
        public string? text;
        public string? ssml;

        // Default text
        public static implicit operator SynthesisInput(string text) => new() { text = text };
    }

    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/VoiceSelectionParams
    /// </summary>
    public record VoiceSelectionParams
    {
        /// <summary>
        /// Required. The language (and potentially also the region) of the voice expressed as a BCP-47 language tag, e.g. "en-US". 
        /// </summary>
        public string languageCode;

        /// <summary>
        /// The name of the voice. If not set, the service will choose a voice based on the other parameters such as languageCode and gender.
        /// </summary>
        public string? name;

        public SsmlVoiceGender? ssmlGender;

        public VoiceSelectionParams(string languageCode, string? name, SsmlVoiceGender? ssmlGender)
        {
            this.languageCode = languageCode;
            this.ssmlGender = ssmlGender;
            this.name = name;
        }
    }

    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/AudioConfig
    /// </summary>
    public record AudioConfig
    {
        /// <summary>
        /// Required. The format of the audio byte stream.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AudioEncoding audioEncoding;

        /// <summary>
        /// Optional. Input only. Speaking rate/speed, in the range [0.25, 4.0]. 1.0 is the normal native speed supported by the specific voice. 2.0 is twice as fast, and 0.5 is half as fast. If unset(0.0), defaults to the native 1.0 speed. Any other values < 0.25 or > 4.0 will return an error.
        /// </summary>
        public float? speakingRate;

        /// <summary>
        /// Optional.Input only.Speaking pitch, in the range [-20.0, 20.0]. 20 means increase 20 semitones from the original pitch. -20 means decrease 20 semitones from the original pitch.
        /// </summary>
        public float? pitch;

        /// <summary>
        /// Optional. Input only. Volume gain (in dB) of the normal native volume supported by the specific voice, in the range [-96.0, 16.0]. If unset, or set to a value of 0.0 (dB), will play at normal native signal amplitude. A value of -6.0 (dB) will play at approximately half the amplitude of the normal native signal amplitude. A value of +6.0 (dB) will play at approximately twice the amplitude of the normal native signal amplitude. Strongly recommend not to exceed +10 (dB) as there's usually no effective increase in loudness for any value greater than that.
        /// </summary>
        public float? volumeGainDb;

        /// <summary>
        /// Optional. The synthesis sample rate (in hertz) for this audio. When this is specified in SynthesizeSpeechRequest, if this is different from the voice's natural sample rate, then the synthesizer will honor this request by converting to the desired sample rate (which might result in worse audio quality), unless the specified sample rate is not supported for the encoding chosen, in which case it will fail the request and return google.rpc.Code.INVALID_ARGUMENT.
        /// </summary>
        public int? sampleRateHertz;

        /// <summary>
        /// Optional. Input only. An identifier which selects 'audio effects' profiles that are applied on (post synthesized) text to speech. Effects are applied on top of each other in the order they are given
        /// </summary>
        public string[]? effectsProfileId;

        public static implicit operator AudioConfig(AudioEncoding audioEncoding) => new() { audioEncoding = audioEncoding, };
    }

    public enum AudioEncoding
    {
        AUDIO_ENCODING_UNSPECIFIED, // Not specified. Will return result google.rpc.Code.INVALID_ARGUMENT.
        LINEAR16, // Uncompressed 16-bit signed little-endian samples (Linear PCM). Audio content returned as LINEAR16 also contains a WAV header.
        MP3, //	MP3 audio at 32kbps.
        MP3_64_KBPS, // MP3 at 64kbps.
        MULAW, // 8-bit samples that compand 14-bit audio samples using G.711 PCMU/mu-law. Audio content returned as MULAW also contains a WAV header.
        OGG_OPUS, // Opus encoded audio wrapped in an ogg container. The result will be a file which can be played natively on Android, and in browsers (at least Chrome and Firefox). The quality of the encoding is considerably higher than MP3 while using approximately the same bitrate.
        ALAW, // 8-bit samples that compand 14-bit audio samples using G.711 PCMU/A-law. Audio content returned as ALAW also contains a WAV header.
    }
}
