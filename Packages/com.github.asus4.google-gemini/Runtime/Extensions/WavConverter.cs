using System;
using System.Buffers;
using System.IO;
using System.Text;
using UnityEngine;

namespace GoogleApis
{
    /// <summary>
    /// Convert AudioClip to Wav
    /// 
    /// Wav format:
    /// https://isip.piconepress.com/projects/speech/software/tutorials/production/fundamentals/v1.0/section_02/s02_01_p05.html
    /// </summary>
    public static class WavConverter
    {
        private const int HEADER_SIZE = 44;
        private static readonly byte[] RIFF = Encoding.ASCII.GetBytes("RIFF");
        private static readonly byte[] WAVE = Encoding.ASCII.GetBytes("WAVE");
        private static readonly byte[] FMT_ = Encoding.ASCII.GetBytes("fmt ");
        private static readonly byte[] DATA = Encoding.ASCII.GetBytes("data");

        public static byte[] ConvertToWav(this AudioClip clip)
        {
            using MemoryStream stream = new();
            ConvertToWav(clip, stream);
            return stream.ToArray();
        }

        public static void ConvertToWav(AudioClip clip, Stream stream)
        {
            const int STRIDE = sizeof(short); // 2 bytes
            int frequency = clip.frequency;
            int channelCount = clip.channels;
            int sampleLength = clip.samples;
            int pcmLength = sampleLength * STRIDE;

            using BinaryWriter writer = new(stream);

            // Write Header
            {
                writer.Write(RIFF); // "RIFF"
                writer.Write(HEADER_SIZE + pcmLength - 8); // File Size - 8
                writer.Write(WAVE); // "WAVE"
                writer.Write(FMT_); // "fmt "
                writer.Write(16); // fmt chunk size
                writer.Write((ushort)1); // PCM format
                writer.Write((ushort)channelCount); // channel count
                writer.Write(frequency); // sample rate
                writer.Write(frequency * channelCount * STRIDE); // byte rate
                writer.Write((ushort)(channelCount * STRIDE)); // block align
                writer.Write((ushort)(STRIDE * 8)); // bits per sample
                writer.Write(DATA); // "data"
                writer.Write(pcmLength); // data size
            }

            // float to 16-bit PCM
            {
                // Get audio data
                float[] floatPool = ArrayPool<float>.Shared.Rent(sampleLength);
                clip.GetData(floatPool, 0);
                ReadOnlySpan<float> floatBuffer = floatPool.AsSpan(0, sampleLength);

                // Prepare PCM data
                byte[] pcmPool = ArrayPool<byte>.Shared.Rent(pcmLength);
                Span<byte> pcmBuffer = pcmPool.AsSpan(0, pcmLength);

                // TODO: running in Burst will be faster?
                for (int i = 0; i < sampleLength; i++)
                {
                    short sample = (short)(floatBuffer[i] * short.MaxValue);
                    pcmBuffer[i * 2] = (byte)(sample & 0xFF);
                    pcmBuffer[i * 2 + 1] = (byte)(sample >> 8);
                }

                // Write PCM data
                writer.Write(pcmBuffer);

                ArrayPool<float>.Shared.Return(floatPool);
                ArrayPool<byte>.Shared.Return(pcmPool);
            }
        }
    }
}
