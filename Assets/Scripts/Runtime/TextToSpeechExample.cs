using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class TextToSpeechExample : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private Button sendButton;

        [SerializeField]
        private Voice voice = Voice.Kore;

        private GenerativeModel model;
        private AudioSource audioSource;
        private CancellationTokenSource cts;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            cts = new CancellationTokenSource();

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);
            // Use the TTS model
            model = client.GetModel(Models.Gemini_2_5_Flash_Preview_TTS);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequestAsync());
            inputField.onSubmit.AddListener(async _ => await SendRequestAsync());
            // for Debug
            inputField.text = "Say cheerfully: Have a wonderful day!";
        }

        private async UniTask SendRequestAsync()
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            inputField.text = string.Empty;

            try
            {
                // Generate speech request
                var request = new GenerateContentRequest()
                {
                    Contents = { new Content(Role.user, input) },
                    GenerationConfig = new GenerationConfig
                    {
                        ResponseModalities = new[] { Modality.AUDIO },
                        SpeechConfig = new SpeechConfig(voice),
                    }
                };

                var response = await model.GenerateContentAsync(request, cts.Token);

                // Extract audio data from response
                var audioData = (response.Candidates?
                    .FirstOrDefault()?.Content?.Parts?
                    .FirstOrDefault(p => p.InlineData != null)?.InlineData)
                    ?? throw new InvalidOperationException("No audio data found in response");

                // Play audio
                if (audioSource.clip != null)
                {
                    Destroy(audioSource.clip);
                }

                // Convert PCM data to AudioClip
                var audioClip = await CreateAudioClipFromPCMAsync(audioData.Data.ToArray(), cts.Token);
                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("Failed to create audio clip from response");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"TTS Error: {e.Message}");
            }
        }

        private async UniTask<AudioClip?> CreateAudioClipFromPCMAsync(byte[] pcmData, CancellationToken cancellationToken)
        {
            await UniTask.SwitchToMainThread(cancellationToken);

            if (pcmData == null || pcmData.Length == 0)
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
            for (int i = 0; i < sampleCount; i++)
            {
                // Convert 16-bit PCM to float (-1 to 1 range)
                short sample = BitConverter.ToInt16(pcmData, i * bytesPerSample);
                floatData[i] = sample / 32768f;
            }

            // Create AudioClip
            var audioClip = AudioClip.Create("TTS_Audio", sampleCount, channels, sampleRate, false);
            audioClip.SetData(floatData, 0);

            return audioClip;
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
