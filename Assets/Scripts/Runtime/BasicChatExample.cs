using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        [SerializeField]
        [TextArea(3, 10)]
        private string message;

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
            Debug.Log($"Client: {client}");

            var models = await client.ListModels(destroyCancellationToken);
            Debug.Log($"Available models: {models}");

            model = client.GetModel("models/gemini-pro");

            // Setup UIs
            {
                sendButton.onClick.AddListener(async () =>
                {
                    var text = inputField.text;
                    if (string.IsNullOrEmpty(text))
                    {
                        return;
                    }
                    // var response = await model.Chat(text);
                    // Debug.Log($"Response: {response}");
                });
            }

        }
    }
}
