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
            {

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

    }
}
