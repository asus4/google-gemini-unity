using System;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    public static class GenerateContentResponseExtension
    {
        public static AudioClip ToAudioClip(this GenerateContentResponse response)
        {
            var blob = response.Candidates
                .FirstOrDefault()?.Content?.Parts?
                .FirstOrDefault(p => p.InlineData != null)?.InlineData;

            if (blob == null)
            {
                throw new InvalidOperationException("No audio data found in response");
            }

            // TODO: Parse mimeType if needed
            // "mimeType": "audio/L16;codec=pcm;rate=24000",
            return CreateAudioClipFromPCM(blob.Data);
        }

        static AudioClip CreateAudioClipFromPCM(ReadOnlyMemory<byte> pcmData, int sampleRate = 24000)
        {
            if (pcmData.Length == 0 || pcmData.Length % 2 != 0)
            {
                throw new ArgumentException("The length of the memory region must be even.", nameof(pcmData));
            }

            const int bytesPerSample = 2; // 16-bit PCM
            int sampleCount = pcmData.Length / bytesPerSample;

            ReadOnlySpan<byte> pcmSpan = pcmData.Span;
            ReadOnlySpan<short> shortSpan = MemoryMarshal.Cast<byte, short>(pcmSpan);

            // Convert byte array to float array
            var floatData = new NativeArray<float>(sampleCount, Allocator.Temp);
            for (int i = 0; i < sampleCount; i++)
            {
                floatData[i] = shortSpan[i] / 32768f;
            }

            // Create AudioClip
            const int channels = 1; // Mono
            var audioClip = AudioClip.Create("TTS_Audio", sampleCount, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);

            return audioClip;
        }
    }
}
