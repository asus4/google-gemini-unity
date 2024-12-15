using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
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
        private static readonly StringBuilder sb = new();

        private async void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // List all available models
            if (showAvailableModels)
            {
                var models = await client.ListModelsAsync(destroyCancellationToken);
                Debug.Log($"Available models: {models}");
            }

            model = client.GetModel(Models.Gemini_2_0_Flash_Exp);

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
            request.safetySettings = new List<SafetySetting>()
            {
                new ()
                {
                    category = HarmCategory.Harassment,
                    threshold = HarmBlockThreshold.Low,
                }
            };

            // Set System prompt if exists
            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                request.systemInstruction = new Content(new Content.Part[] { systemInstruction });
            }

            if (useStream)
            {
                await model.StreamGenerateContentAsync(request, destroyCancellationToken, (response) =>
                {
                    if (response.candidates.Length == 0)
                    {
                        return;
                    }
                    // Merge to last message if the role is the same
                    Content streamContent = response.candidates[0].content;
                    bool mergeToLast = messages.Count > 0
                        && messages[^1].role == streamContent.role;
                    if (mergeToLast)
                    {
                        messages[^1] = MergeContent(messages[^1], streamContent);
                    }
                    else
                    {
                        messages.Add(streamContent);
                    }
                    RefreshView();
                });
            }
            else
            {
                var response = await model.GenerateContentAsync(request, destroyCancellationToken);
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
            foreach (var message in messages)
            {
                sb.AppendTMPRichText(message);
            }
            messageLabel.SetText(sb);
        }

        private static Content MergeContent(Content a, Content b)
        {
            if (a.role != b.role)
            {
                return null;
            }

            sb.Clear();
            var parts = new List<Content.Part>();
            foreach (var part in a.parts)
            {
                if (string.IsNullOrWhiteSpace(part.text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.text);
                }
            }
            foreach (var part in b.parts)
            {
                if (string.IsNullOrWhiteSpace(part.text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.text);
                }
            }
            parts.Insert(0, sb.ToString());
            return new Content(a.role.Value, parts.ToArray());
        }
    }
}
