using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Gemini
{
    /// <summary>
    /// Function Calling Example
    /// 
    /// https://ai.google.dev/docs/function_calling
    /// https://github.com/google-gemini/cookbook/blob/main/quickstarts/rest/Function_calling_REST.ipynb
    /// </summary>
    public sealed class FunctionCallingExample : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI messageLabel;

        [SerializeField]
        private Button sendButton;


        private GenerativeModel model;
        private readonly List<Content> messages = new();
        private static readonly StringBuilder sb = new();

        private Tool[] tools;

        private void Start()
        {
            using var settings = GenerativeAISettings.Get();
            var client = new GenerativeAIClient(settings);

            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "I have 57 cats, each owns 44 mittens, how many mittens is that in total?";

            tools = new Tool[] { this.BuildFunctionsFromAttributes() };
            Debug.Log($"tools:{tools.First()}");
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

            // 1. Make request with Tools
            GenerateContentRequest request = new()
            {
                contents = messages,
                tools = tools,
            };
            var response1 = await model.GenerateContentAsync(request, destroyCancellationToken);

            // 2. Receive function call response
            var modelContent = response1.candidates.First().content;
            messages.Add(modelContent);
            RefreshView();

            // 3. Invoke function call in local client
            var functionCall = modelContent.FindFunctionCall();
            Assert.IsNotNull(functionCall);
            var result = this.InvokeFunctionCall(functionCall);

            // 4. Send function response to model
            Content.FunctionResponse functionResponse = new(functionCall.name, result);
            messages.Add(new(Role.Function, functionResponse));
            RefreshView();

            // 5. Generate content with function response
            var response2 = await model.GenerateContentAsync(request, destroyCancellationToken);
            messages.Add(response2.candidates.First().content);
            RefreshView();
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

        #region Function Calls
        [Preserve]
        [FunctionCall("Return a + b.")]
        public float Add([FunctionCall("A")] float a, [FunctionCall("B")] float b)
        {
            return a + b;
        }

        [Preserve]
        [FunctionCall("Return a - b.")]
        public static float Subtract(float a, float b)
        {
            return a - b;
        }

        [Preserve]
        [FunctionCall("Return a * b.")]
        public float Multiply(float a, float b)
        {
            return a * b;
        }

        [Preserve]
        [FunctionCall("Return a / b.")]
        public float Divide(float a, float b)
        {
            return a / b;
        }
        #endregion Function Calls
    }
}
