using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Imagen 3.0 image generation example
    /// https://github.com/google-gemini/cookbook/blob/main/quickstarts/Get_started_imagen_rest.ipynb
    /// </summary>
    public sealed class ImagenExample : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        private RawImage resultImage;

        [SerializeField]
        private GenerateImageRequest.AspectRatioEnum aspectRatio;

        private GenerativeModel model;

        private void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            model = client.GetModel(Models.Imagen_3_0);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "A hairy bunny in my kitchen playing with a tomato.";
        }

        private async Task SendRequest()
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            GenerateImageRequest request = new(input, new GenerateImageRequest.ImageGenerationParameters()
            {
                SampleCount = 1,
                AspectRatioEnum = aspectRatio,
                PersonGeneration = GenerateImageRequest.PersonGeneration.allow_adult,
            });

            var response = await model.GenerateImageAsync(request, destroyCancellationToken);
            // Set texture
            if (resultImage.texture != null)
            {
                Destroy(resultImage.texture);
            }
            var texture = response.Predictions[0].ToTexture();
            resultImage.texture = texture;
            if (resultImage.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
            {
                aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            }
        }
    }
}
