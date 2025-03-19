using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.AppUI.UI;
using LlmUI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Basic Chat Example
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    sealed class BasicChatExampleUI : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField]
        [TextArea(1, 10)]
        string systemInstruction = string.Empty;

        [SerializeField]
        bool showAvailableModels;

        [SerializeField]
        bool useStream = false;

        [SerializeField]
        bool enableSearch = true;

        [SerializeField]
        List<ContentItem> contentItems;

        GenerativeModel model;
        readonly List<Content> messages = new();
        static readonly StringBuilder sb = new();

        async void Start()
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

            // Setup UI
            if (!TryGetComponent(out UIDocument document))
            {
                Debug.LogError("UIDocument is missing");
                return;
            }

            var root = document.rootVisualElement;
            var promptSendButton = root.Q<IconButton>("prompt-send-button");
            promptSendButton.clicked += async () => await SendRequest();

            var contentListView = root.Q<ListView>("content-list-view");
            contentListView.itemsSource = contentItems;
        }

        async Task SendRequest()
        {
            Debug.Log("SendRequest");
            return;
            // var input = inputField.text;
            var input = "How are you?";
            // if (string.IsNullOrEmpty(input))
            // {
            //     return;
            // }
            // inputField.text = string.Empty;

            Content content = new(Role.user, input);
            messages.Add(content);
            RefreshView();

            GenerateContentRequest request = messages;
            if (enableSearch)
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

        void RefreshView()
        {
            sb.Clear();
            foreach (var message in messages)
            {
                sb.AppendTMPRichText(message);
            }

            Debug.Log($"TODO: Set Text Here: {sb}");
            // messageLabel.SetText(sb);
        }

        static Content MergeContent(Content a, Content b)
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
