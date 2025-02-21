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

        [SerializeField]
        private bool enableSearch = true;


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

            model = client.GetModel(Models.Gemini_2_0_Flash);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "What is the weather like in Berlin tomorrow?";
        }

        private async Task SendRequest()
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            inputField.text = string.Empty;

            Content content = new(Role.user, input);
            messages.Add(content);
            RefreshView();

            GenerateContentRequest request = messages;
            if(enableSearch)
            {
                request.Tools = new Tool[]
                {
                    new Tool.GoogleSearchRetrieval(),
                };
            }
            request.SafetySettings = new List<SafetySetting>()
            {
                new ()
                {
                    category = HarmCategory.HARM_CATEGORY_HARASSMENT,
                    threshold = HarmBlockThreshold.BLOCK_LOW_AND_ABOVE,
                }
            };

            // Set System prompt if exists
            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                request.SystemInstruction = new Content(new Part[] { systemInstruction });
            }

            if (useStream)
            {
                var stream = model.StreamGenerateContentAsync(request, destroyCancellationToken);
                await foreach (var response in stream)
                {
                    if (response.Candidates.Length == 0)
                    {
                        return;
                    }
                    // Merge to last message if the role is the same
                    Content streamContent = response.Candidates[0].Content;
                    bool mergeToLast = messages.Count > 0
                        && messages[^1].Role == streamContent.Role;
                    if (mergeToLast)
                    {
                        messages[^1] = MergeContent(messages[^1], streamContent);
                    }
                    else
                    {
                        messages.Add(streamContent);
                    }
                    RefreshView();
                }
            }
            else
            {
                var response = await model.GenerateContentAsync(request, destroyCancellationToken);
                if (response.Candidates.Length > 0)
                {
                    var modelContent = response.Candidates[0].Content;
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
            if (a.Role != b.Role)
            {
                return null;
            }

            sb.Clear();
            var parts = new List<Part>();
            foreach (var part in a.Parts)
            {
                if (string.IsNullOrWhiteSpace(part.Text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.Text);
                }
            }
            foreach (var part in b.Parts)
            {
                if (string.IsNullOrWhiteSpace(part.Text))
                {
                    parts.Add(part);
                }
                else
                {
                    sb.Append(part.Text);
                }
            }
            parts.Insert(0, sb.ToString());
            return new Content(a.Role.Value, parts.ToArray());
        }
    }
}
