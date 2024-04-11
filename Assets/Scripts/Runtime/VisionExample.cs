using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gemini
{
    public sealed class VisionExample : MonoBehaviour
    {
        [SerializeField]
        private Texture inputTexture;

        [SerializeField]
        [Multiline(10)]
        private string inputText;

        [SerializeField]
        private TextMeshProUGUI resultLabel;

        [SerializeField]
        private Button sendButton;

        private GenerativeModel model;

        private void Start()
        {
            using var settings = GenerativeAISettings.Get();
            var client = new GenerativeAIClient(settings);
            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
        }

        private async Task SendRequest()
        {
            Debug.Log("SendRequest");
            var blob = await inputTexture.ToJpgBlobAsync();
            Debug.Log($"Blob mime: {blob.mimeType}, data: {blob.data}");
        }
    }
}
