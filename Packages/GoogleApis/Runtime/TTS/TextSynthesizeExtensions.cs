using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleApis.TTS
{
    /// <summary>
    /// Utility methods for <see cref="TextSynthesizeResponse"/>.
    /// </summary>
    public static class TextSynthesizeExtensions
    {
        public static async Task<AudioClip> ToAudioClipAsync(
            this TextSynthesizeResponse response, CancellationToken cancellationToken)
        {
            // Save base64 audio to a tmp file
            AudioEncoding encoding = response.audioConfig.audioEncoding;
            string tempPath = Path.Combine(Application.temporaryCachePath, $"{Guid.NewGuid()}{encoding.ToExtension()}");
            byte[] bytes = Convert.FromBase64String(response.audioContent);
            await File.WriteAllBytesAsync(tempPath, bytes, cancellationToken);
            // Debug.Log($"Saved to {tempPath}");

            // Load audio clip from the file
            using var request = UnityWebRequestMultimedia.GetAudioClip(
                $"file://{tempPath}", encoding.ToAudioType());
            await request.SendWebRequest();

            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }

            return DownloadHandlerAudioClip.GetContent(request);
        }

        private static string ToExtension(this AudioEncoding encoding)
        {
            return encoding switch
            {
                AudioEncoding.LINEAR16 => ".wav",
                AudioEncoding.MP3 => ".mp3",
                AudioEncoding.MP3_64_KBPS => ".mp3",
                AudioEncoding.MULAW => ".wav",
                AudioEncoding.OGG_OPUS => ".ogg",
                AudioEncoding.ALAW => ".wav",
                _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null),
            };
        }

        private static AudioType ToAudioType(this AudioEncoding encoding)
        {
            return encoding switch
            {
                AudioEncoding.LINEAR16 => AudioType.WAV,
                AudioEncoding.MP3 => AudioType.MPEG,
                AudioEncoding.MP3_64_KBPS => AudioType.MPEG,
                AudioEncoding.MULAW => AudioType.WAV,
                AudioEncoding.OGG_OPUS => AudioType.OGGVORBIS,
                AudioEncoding.ALAW => AudioType.WAV,
                _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null),
            };
        }
    }
}
