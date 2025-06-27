using System;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    public static class GenerateContentResponseExtension
    {
        /// <summary>
        /// Convert GenerateContentResponse to AudioClip.
        /// </summary>
        /// <param name="response">A <see cref="GenerateContentResponse"/> object.</param>
        /// <returns>An AudioClip created from the response.</returns>
        /// <exception cref="InvalidOperationException"></exception>
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
                throw new ArgumentException("PCM data length must be even.", nameof(pcmData));
            }

            const int bytesPerSample = 2;
            int sampleCount = pcmData.Length / bytesPerSample;
            const int channels = 1; // Mono

            // Cast byte -> short
            ReadOnlySpan<short> pcmShortSpan = MemoryMarshal.Cast<byte, short>(pcmData.Span);

            // Prepare Job resources
            using var pcmNativeArray = new NativeArray<short>(pcmShortSpan.ToArray(), Allocator.TempJob);
            using var floatData = new NativeArray<float>(sampleCount, Allocator.TempJob);
            var job = new ConvertShortToFloatJob
            {
                PcmData = pcmNativeArray,
                FloatData = floatData
            };
            job.Run();

            var audioClip = AudioClip.Create("TTS_Audio", sampleCount, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);

            return audioClip;
        }

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct ConvertShortToFloatJob : IJob
        {
            [ReadOnly]
            public NativeArray<short> PcmData;

            [WriteOnly]
            public NativeArray<float> FloatData;

            public void Execute()
            {
                const float SCALE = 1.0f / 32768.0f;
                for (int i = 0; i < PcmData.Length; i++)
                {
                    FloatData[i] = PcmData[i] * SCALE;
                }
            }
        }
    }
}
