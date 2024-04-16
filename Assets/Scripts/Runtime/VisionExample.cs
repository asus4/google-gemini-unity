using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gemini
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
            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
        }

        private async Task SendRequest()
        {
            var blob = await inputTexture.ToJpgBlobAsync();

            Content[] messages = { new(Role.User, blob, inputText) };
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
