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
        [Header("UI references")]
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI messageLabel;

        [SerializeField]
        private Button sendButton;

        [Header("Options")]
        [SerializeField]
        [TextArea(1, 10)]
        private string systemInstruction = string.Empty;

        [SerializeField]
        private bool showAvailableModels;

        [SerializeField]
        private bool useStream = false;


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
            inputField.text = "Write a story about a cat and a dog.";
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
            messages.Add(content);
            RefreshView();

            GenerateContentRequest request = messages;

            // Set System prompt if exists
            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                request.systemInstruction = new Content(systemInstruction);
            }


            if (useStream)
            {
                await model.StreamGenerateContentAsync(request, destroyCancellationToken, (response) =>
                {
                    Debug.Log($"Response: {response}");
                    if (response.candidates.Length > 0)
                    {
                        var modelContent = response.candidates[0].content;
                        messages.Add(modelContent);
                        RefreshView();
                    }
                });
            }
            else
            {
                var response = await model.GenerateContentAsync(request, destroyCancellationToken);
                Debug.Log($"Response: {response}");
                if (response.candidates.Length > 0)
                {
                    var modelContent = response.candidates[0].content;
                    messages.Add(modelContent);
                    RefreshView();
                }
            }
        }

        private void RefreshView()
        {
            sb.Clear();
            if (messages.Count == 0)
            {
                messageLabel.SetText(sb);
                return;
            }

            Role role = Role.User;
            for (int i = 0; i < messages.Count; i++)
            {
                Content content = messages[i];

                // Display only role changed from previous
                bool needDisplayRole = i == 0
                    || (content.role.HasValue && content.role.Value != role);
                if (needDisplayRole)
                {
                    sb.AppendLine($"<b>{content.role}:</b>");
                    role = content.role.Value;
                }
                // Display content
                foreach (var part in content.parts)
                {
                    if (!string.IsNullOrWhiteSpace(part.text))
                    {
                        sb.AppendLine(part.text.MarkdownToRichText());
                    }
                    else
                    {
                        sb.AppendLine($"<color=red>Unsupported part</color>");
                    }
                }
            }
            // Set to label
            messageLabel.SetText(sb);
        }
    }
}
