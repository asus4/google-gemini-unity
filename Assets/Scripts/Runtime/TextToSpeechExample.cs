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

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);
            // Should use the TTS model
            model = client.GetModel(Models.Gemini_2_5_Flash_Preview_TTS);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequestAsync(destroyCancellationToken));
            inputField.onSubmit.AddListener(async _ => await SendRequestAsync(destroyCancellationToken));
            // for Debug
            inputField.text = "Say cheerfully: Have a wonderful day!";
        }

        async UniTask SendRequestAsync(CancellationToken cancellationToken)
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            inputField.text = string.Empty;

            Debug.Log($"TTS Input: {input}");

            // Generate request
            var request = MakeSpeechRequest(input, voice);
            Debug.Log($"TTS Request: {request}");

            try
            {
                var response = await model.GenerateContentAsync(request, cancellationToken);
                await UniTask.SwitchToMainThread(cancellationToken);

                // Delete the previous audio clip if it exists
                if (audioSource.clip != null)
                {
                    Destroy(audioSource.clip);
                }

                audioSource.clip = response.ToAudioClip();
                audioSource.Play();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"TTS Error: {e.Message}");
            }
        }

        static GenerateContentRequest MakeSpeechRequest(string input, Voice voice)
        {
            return new GenerateContentRequest()
            {
                Contents = new Content[]
                {
                    new (new Part[] { input })
                },
                GenerationConfig = new()
                {
                    ResponseModalities = new[] { Modality.AUDIO },
                    SpeechConfig = new(voice),
                }
            };
        }
    }
}
