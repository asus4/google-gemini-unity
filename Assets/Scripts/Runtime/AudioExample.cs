using System.Text;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Audio understanding example introduced in Gemini 1.5 Pro
    /// </summary>
    public sealed class AudioExample : MonoBehaviour
    {
        [SerializeField]
        private AudioClip audioClip;

        [SerializeField]
        private TextMeshProUGUI resultLabel;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        [Multiline(10)]
        private string inputText;

        private readonly StringBuilder sb = new();
        private GenerativeModel model;

        private void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            model = client.GetModel(Models.Gemini_2_0_Flash);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
        }

        private async Task SendRequest()
        {
            // Add audio data to the message
            byte[] audioData = audioClip.ConvertToWav();
            var blob = new Content.Blob("audio/wav", audioData);
            Content[] messages = { new(Role.User, inputText, blob), };

            sb.AppendTMPRichText(messages[0]);
            resultLabel.SetText(sb);

            var response = await model.GenerateContentAsync(messages, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.candidates.Length > 0)
            {
                var modelContent = response.candidates[0].content;
                sb.AppendTMPRichText(modelContent);
                resultLabel.SetText(sb);
            }
        }
    }
}
