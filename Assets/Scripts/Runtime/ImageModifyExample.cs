using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GoogleApis.Example
{
    public sealed class ImageModifyExample : MonoBehaviour
    {
        [SerializeField]
        RawImage inputImage;

        [SerializeField]
        RawImage outputImage;

        [SerializeField]
        TMP_InputField inputField;

        [SerializeField]
        Button sendButton;

        GenerativeModel model;


        void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);
            model = client.GetModel(Models.Gemini_2_0_Flash);

            sendButton.onClick.AddListener(async () => await SendRequest());
        }

        async UniTask SendRequest()
        {
            string prompt = inputField.text;
            if (string.IsNullOrEmpty(prompt))
            {
                return;
            }

            // Disable the button to prevent multiple requests
            sendButton.interactable = false;
            try
            {
                outputImage.texture = await EditImageAsync(inputImage.texture, prompt);
            }
            finally
            {
                sendButton.interactable = true;
            }
        }

        async UniTask<Texture> EditImageAsync(Texture texture, string prompt)
        {
            var blob = await texture.ToJpgBlobAsync();

            Content[] messages = { new(Role.user, blob, prompt) };
            GenerateContentRequest request = new()
            {
                Contents = messages,
                GenerationConfig = new()
                {
                    ResponseModalities = new[] { Modality.IMAGE },
                },
            };

            var response = await model.GenerateContentAsync(request, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.Candidates.Length == 0)
            {
                return null;
            }

            var modelContent = response.Candidates[0].Content;
            // TODO : parse content
            return null;
        }
    }
}
