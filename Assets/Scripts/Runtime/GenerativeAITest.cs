using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace GenerativeAI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GenerativeAITest : MonoBehaviour
    {
        [SerializeField]
        [TextArea(3, 10)]
        private string message;

        private GenerativeAIClient client;

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

            var settings = GenerativeAISettings.Get();
            client = new GenerativeAIClient(settings.apiKey);
            Debug.Log($"Client: {client}");

            var models = await client.ListModels(destroyCancellationToken);
            Debug.Log($"Available models: {models}");
        }

        private void OnDestroy()
        {
            // TODO: uninitialized client
        }

        private void SetupUI(UIDocument document)
        {
            var root = document.rootVisualElement;
            var promptTextField = root.Query<TextField>("promptTextField");
        }
    }
}
