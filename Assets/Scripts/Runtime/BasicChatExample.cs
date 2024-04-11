using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gemini
{
    /// <summary>
    /// Basic Chat Example
    /// </summary>
    public sealed class BasicChatExample : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI historyLabel;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        private bool showAvailableModels;

        private GenerativeModel model;

        private readonly List<Content> messages = new();
        private readonly StringBuilder sb = new();

        private async void Start()
        {
            using var settings = GenerativeAISettings.Get();
            var client = new GenerativeAIClient(settings);

            // List all available models
            if (showAvailableModels)
            {
                var models = await client.ListModelsAsync(destroyCancellationToken);
                Debug.Log($"Available models: {models}");
            }

            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "Hello! what is your name?";
        }

        private async Task SendRequest()
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            inputField.text = string.Empty;

            Content content = new(Role.User, input);
            AppendToView(content);
            messages.Add(content);

            var response = await model.GenerateContentAsync(messages, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.candidates.Length > 0)
            {
                var modelContent = response.candidates[0].content;
                AppendToView(modelContent);
                messages.Add(modelContent);
            }
        }

        private void AppendToView(Content content)
        {
            sb.AppendLine($"<b>{content.role}:</b>");
            foreach (var part in content.parts)
            {
                if (part.text != null)
                {
                    sb.AppendLine(part.text);
                }
                else
                {
                    sb.AppendLine($"<color=red>Unsupported part</color>");
                }
            }
            historyLabel.SetText(sb);
        }
    }
}
