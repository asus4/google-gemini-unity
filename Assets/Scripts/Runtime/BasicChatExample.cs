using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GenerativeAI
{
    /// <summary>
    /// Basic Chat Examples
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class BasicChatExample : MonoBehaviour
    {
        [SerializeField]
        [TextArea(3, 10)]
        private string message;

        private GenerativeAIClient client;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }

        private async void Start()
        {
            // Setup UIs
            if (TryGetComponent<UIDocument>(out var document))
            {
                SetupUI(document);
            }
            else
            {
                throw new Exception("UIDocument not found");
            }

            using var settings = GenerativeAISettings.Get();
            client = new GenerativeAIClient(settings.apiKey);
            Debug.Log($"Client: {client}");

            // var models = await client.ListModels(destroyCancellationToken);
            // Debug.Log($"Available models: {models}");
        }

        private void OnDestroy()
        {
            // TODO: uninitialized client
        }

        private void SetupUI(UIDocument document)
        {
            var root = document.rootVisualElement;
            var promptTextField = root.Query<TextField>("promptTextField");
            Debug.Log($"PromptTextField: {promptTextField}");
        }
    }
}
