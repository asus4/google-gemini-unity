using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeAI
{
    /// <summary>
    /// Basic Chat Examples
    /// </summary>
    public sealed class BasicChatExample : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Button sendButton;

        private GenerativeModel model;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }

        private async void Start()
        {
            using var settings = GenerativeAISettings.Get();
            var client = new GenerativeAIClient(settings);

            // List all available models
            if (Application.isEditor)
            {
                var models = await client.ListModels(destroyCancellationToken);
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

            // TODO: add chat history
            var response = await model.GenerateContent(new(input), destroyCancellationToken);
            Debug.Log($"Response: {response}");
        }
    }
}
