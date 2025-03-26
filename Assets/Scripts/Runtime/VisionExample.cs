using System.Text;
using Cysharp.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Send image example
    /// </summary>
    public sealed class VisionExample : MonoBehaviour
    {
        [SerializeField]
        private Texture inputTexture;

        [SerializeField]
        [Multiline(10)]
        private string inputText;

        [SerializeField]
        private TextMeshProUGUI resultLabel;

        [SerializeField]
        private Button sendButton;

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

        private async UniTask SendRequest()
        {
            var blob = await inputTexture.ToJpgBlobAsync();

            Content[] messages = { new(Role.user, blob, inputText) };
            sb.AppendTMPRichText(messages[0]);
            resultLabel.SetText(sb);

            GenerateContentRequest request = new()
            {
                Contents = messages,
                Tools = new Tool[]
                {
                    new Tool.GoogleSearchRetrieval(),
                },
            };

            var response = await model.GenerateContentAsync(request, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.Candidates.Length > 0)
            {
                sb.AppendTMPRichText(response.Candidates[0]);
                resultLabel.SetText(sb);
            }
        }
    }
}
