using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Function Calling Example
    /// 
    /// https://ai.google.dev/docs/function_calling
    /// https://github.com/google-gemini/cookbook/blob/main/quickstarts/rest/Function_calling_REST.ipynb
    /// </summary>
    public sealed class FunctionCallingExample : MonoBehaviour
    {
        public enum ModelType
        {
            Gemini_2_0_Flash,
            Gemini_2_0_Pro,
        }

        public enum ToolMode
        {
            WorldBuilder,
            WebContent
        }

        [Header("Scene references")]
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI messageLabel;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        private ModelType modelType = ModelType.Gemini_2_0_Flash;

        [SerializeField]
        private ToolMode toolType = ToolMode.WorldBuilder;

        [SerializeField]
        [TextArea(1, 10)]
        private string systemInstruction = "You are a helpful assistant in Unity GameEngine.";

        private GenerativeModel model;
        private readonly List<Content> messages = new();
        private static readonly StringBuilder sb = new();

        private Content systemInstructionContent;
        private MonoBehaviour toolInstance;
        private Tool[] tools;

        private void Start()
        {
            // Make Gemini model
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // Use 1.0 as 1.5 is rate limited in 5/minute, or increase the rate limit of 1.5
            string modelName = modelType switch
            {
                ModelType.Gemini_2_0_Flash => Models.Gemini_2_0_Flash,
                ModelType.Gemini_2_0_Pro => Models.Gemini_2_0_Pro_Exp,
                _ => throw new System.NotImplementedException(),
            };
            model = client.GetModel(modelName);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // Set system instruction
            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                systemInstructionContent = new Content(new Content.Part[] { systemInstruction });
            }

            // Build Tools from all [FunctionCall("description")] attributes in the script.
            toolInstance = toolType switch
            {
                ToolMode.WorldBuilder => gameObject.AddComponent<FunctionCallWorldBuilder>(),
                ToolMode.WebContent => gameObject.AddComponent<FunctionCallWebContent>(),
                _ => throw new System.NotImplementedException(),
            };
            tools = new Tool[] { toolInstance.BuildFunctionsFromAttributes() };
            Debug.Log($"tools:\n{tools.First()}");

            // for Debug
            inputField.text = toolType switch
            {
                ToolMode.WorldBuilder => "Make a big floor then make a cube on it.",
                ToolMode.WebContent => "Summarize the following webpage in three sentence: https://en.wikipedia.org/wiki/Unity_(game_engine)",
                _ => throw new System.NotImplementedException(),
            };
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

            while (true)
            {
                // 1. Make request with Tools
                GenerateContentRequest request = new()
                {
                    contents = messages,
                    tools = tools,
                };
                if (systemInstructionContent != null)
                {
                    request.systemInstruction = systemInstructionContent;
                }

                // 2. Receive response
                var response = await model.GenerateContentAsync(request, destroyCancellationToken);
                var modelContent = response.candidates.First().content;
                messages.Add(modelContent);
                RefreshView();

                // Stop if no function call in the response
                if (!modelContent.ContainsFunctionCall())
                {
                    return;
                }

                // 3. Invoke function call in local client
                Content functionResponseContent = await toolInstance.InvokeFunctionCallsAsync(
                    modelContent, destroyCancellationToken);

                // 4. Send function response back to model
                messages.Add(functionResponseContent);
                RefreshView();
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
    }
}
