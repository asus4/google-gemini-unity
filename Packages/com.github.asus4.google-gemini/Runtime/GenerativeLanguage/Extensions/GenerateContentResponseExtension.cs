using System;
using System.Linq;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    public static class GenerateContentResponseExtension
    {
        public static AudioClip ToAudioClip(this GenerateContentResponse response)
        {
            var audioData = response.Candidates
                .FirstOrDefault()?.Content?.Parts?
                .FirstOrDefault(p => p.InlineData != null)?.InlineData;

            if (audioData == null)
            {
                throw new InvalidOperationException("No audio data found in response");
            }

            return CreateAudioClipFromPCM(audioData.Data);
        }

        static AudioClip CreateAudioClipFromPCM(ReadOnlyMemory<byte> pcmData)
        {

            if (pcmData.Length == 0)
            {
                Debug.LogError("No audio data available");
                return null;
            }

            const int bytesPerSample = 2; // 16-bit
            const int sampleRate = 24000; // TTS models output 24kHz audio
            const int channels = 1; // Mono
            int sampleCount = pcmData.Length / bytesPerSample;

            // Convert byte array to float array
            float[] floatData = new float[sampleCount];
            ReadOnlySpan<byte> span = pcmData.Span;
            for (int i = 0; i < sampleCount; i++)
            {
                // Convert 16-bit PCM to float (-1 to 1 range)
                short sample = BitConverter.ToInt16(span.Slice(i * bytesPerSample, bytesPerSample));
                floatData[i] = sample / 32768f;
            }

            // Create AudioClip
            var audioClip = AudioClip.Create("TTS_Audio", sampleCount, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);

            return audioClip;
        }
    }

}
