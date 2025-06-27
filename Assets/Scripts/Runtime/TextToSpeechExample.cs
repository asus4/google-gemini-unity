using System.Collections.Generic;
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
        private TMP_Dropdown voiceDropdown;

        private GenerativeModel ttsModel;
        private AudioSource audioSource;
        private CancellationTokenSource cts;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            cts = new CancellationTokenSource();

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);
            // Use the TTS model
            ttsModel = client.GetModel(Models.Gemini_2_5_Flash_Preview_TTS);

            // Setup voice dropdown if available
            if (voiceDropdown != null)
            {
                voiceDropdown.ClearOptions();
                voiceDropdown.AddOptions(new List<string>
                { 
                    "Kore", "Aoede", "Charon", "Fenrir", "Orpheus", "Puck" 
                });
                voiceDropdown.value = 0; // Default to Kore
            }

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
                // Get selected voice
                string voiceName = voiceDropdown != null && voiceDropdown.options.Count > 0 
                    ? voiceDropdown.options[voiceDropdown.value].text 
                    : "Kore";

                // Generate speech
                var request = new GenerateSpeechRequest(
                    text: input,
                    voiceName: voiceName
                );
                
                var response = await ttsModel.GenerateSpeechAsync(request, cts.Token);
                Debug.Log($"TTS response received");

                // Play audio
                if (audioSource.clip != null)
                {
                    Destroy(audioSource.clip);
                }
                
                var audioClip = await response.ToAudioClipAsync(cts.Token);
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

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
