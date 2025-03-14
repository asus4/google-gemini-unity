using System.Linq;
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
            model = client.GetModel(Models.Gemini_2_0_Flash_Exp);

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
                var tex = await EditImageAsync(inputImage.texture, prompt);
                SetImage(outputImage, tex);
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
                    ResponseModalities = new[] { Modality.TEXT, Modality.IMAGE },
                },
            };

            var response = await model.GenerateContentAsync(request, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.Candidates.Length == 0)
            {
                return null;
            }

            var inlineData = response.Candidates[0].Content.Parts
                .Where(part
                    => part.InlineData != null && part.InlineData.MimeType.StartsWith("image/"))
                .Select(part => part.InlineData)
                .FirstOrDefault();

            return inlineData?.ToTexture();
        }


        static void SetImage(RawImage image, Texture texture)
        {
            image.texture = texture;
            if (image.TryGetComponent<AspectRatioFitter>(out var aspectRatioFitter))
            {
                aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            }
        }
    }
}
