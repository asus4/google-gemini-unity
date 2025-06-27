#nullable enable

using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Request for generating speech from text using Gemini TTS models.
    /// </summary>
    public record GenerateSpeechRequest
    {
        /// <summary>
        /// The text content to convert to speech.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The voice to use for speech generation.
        /// </summary>
        [JsonPropertyName("voiceName")]
        public string VoiceName { get; set; }

        /// <summary>
        /// Optional speech configuration for advanced settings.
        /// </summary>
        [JsonPropertyName("speechConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SpeechConfig? SpeechConfig { get; set; }

        public GenerateSpeechRequest(string text, string voiceName = "Kore", SpeechConfig? speechConfig = null)
        {
            Text = text;
            VoiceName = voiceName;
            SpeechConfig = speechConfig;
        }

        /// <summary>
        /// Converts this request to a GenerateContentRequest for the Gemini API.
        /// </summary>
        internal GenerateContentRequest ToGenerateContentRequest()
        {
            return new GenerateContentRequest
            {
                Contents = new[]
                {
                    new Content(new Part[] { new Part { Text = Text } })
                },
                GenerationConfig = new GenerationConfig
                {
                    ResponseModalities = new[] { Modality.AUDIO },
                    SpeechConfig = SpeechConfig ?? new SpeechConfig
                    {
                        VoiceConfig = new VoiceConfig
                        {
                            PrebuiltVoiceConfig = new PrebuiltVoiceConfig(VoiceName)
                        }
                    }
                }
            };
        }
    }

    /// <summary>
    /// Response from speech generation containing audio data.
    /// </summary>
    public record GenerateSpeechResponse
    {
        /// <summary>
        /// The audio data as PCM bytes (24kHz, 16-bit, mono).
        /// </summary>
        [JsonPropertyName("audioData")]
        public byte[] AudioData { get; set; }

        /// <summary>
        /// The MIME type of the audio data.
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        /// <summary>
        /// The sample rate of the audio.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public int SampleRate { get; set; }

        /// <summary>
        /// The number of audio channels.
        /// </summary>
        [JsonPropertyName("channels")]
        public int Channels { get; set; }

        public GenerateSpeechResponse(byte[] audioData, string mimeType = "audio/pcm", int sampleRate = 24000, int channels = 1)
        {
            AudioData = audioData;
            MimeType = mimeType;
            SampleRate = sampleRate;
            Channels = channels;
        }

        /// <summary>
        /// Creates a GenerateSpeechResponse from a GenerateContentResponse.
        /// </summary>
        internal static GenerateSpeechResponse FromGenerateContentResponse(GenerateContentResponse response)
        {
            // Extract audio data from the response
            var audioData = response.Candidates?
                .FirstOrDefault()?.Content?.Parts?
                .FirstOrDefault(p => p.InlineData != null)?.InlineData;

            if (audioData == null)
            {
                throw new InvalidOperationException("No audio data found in response");
            }

            // Convert base64 to byte array
            var pcmData = audioData.Data.ToArray();

            return new GenerateSpeechResponse(
                audioData: pcmData,
                mimeType: audioData.MimeType ?? "audio/pcm",
                sampleRate: 24000,
                channels: 1
            );
        }

        /// <summary>
        /// Converts the audio data to a Unity AudioClip.
        /// </summary>
        public async UniTask<AudioClip?> ToAudioClipAsync(CancellationToken cancellationToken = default)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            if (AudioData == null || AudioData.Length == 0)
            {
                Debug.LogError("No audio data available");
                return null;
            }

            const int bytesPerSample = 2; // 16-bit
            int sampleCount = AudioData.Length / bytesPerSample;

            // Convert byte array to float array
            float[] floatData = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                // Convert 16-bit PCM to float (-1 to 1 range)
                short sample = BitConverter.ToInt16(AudioData, i * bytesPerSample);
                floatData[i] = sample / 32768f;
            }

            // Create AudioClip
            var audioClip = AudioClip.Create("TTS_Audio", sampleCount, Channels, SampleRate, false);
            audioClip.SetData(floatData, 0);

            return audioClip;
        }

        public override string ToString() => this.SerializeToJson(true);
    }
}
