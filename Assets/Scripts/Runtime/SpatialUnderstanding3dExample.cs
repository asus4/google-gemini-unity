using System;
using System.Collections.Generic;
using System.Linq;
using GoogleApis.GenerativeLanguage;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Example of Spatial Understanding 3D (experimental).
    /// 
    /// Original source:
    /// https://github.com/google-gemini/cookbook/blob/main/examples/Spatial_understanding_3d.ipynb
    /// https://github.com/google-gemini/starter-applets/blob/main/spatial/src/Content.tsx
    /// </summary>
    public sealed class SpatialUnderstanding3dExample : MonoBehaviour
    {
        [Serializable]
        class Box3d
        {
            public string label;
            public float[] box_3d;
        }

        [SerializeField]
        private Texture inputTexture;

        [SerializeField]
        private RawImage rawImage;

        [SerializeField]
        [TextArea]
        private string inputText = "Output in json. Detect the 3D bounding boxes of items , output no more than 10 items. Return a list where each entry contains the object name in \"label\" and its 3D bounding box in \"box_3d\".";

        [SerializeField]
        [Range(10f, 120f)]
        private float fieldOfView = 69f;

        [SerializeField]
        private bool isTest = true;

        [SerializeField]
        Box3d[] results;

        private GenerativeModel model;

        private async void Start()
        {
            // Set image
            rawImage.texture = inputTexture;
            if (rawImage.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
            {
                aspectRatioFitter.aspectRatio = (float)inputTexture.width / inputTexture.height;
            }

            if (isTest)
            {
                string text = "```json\n[\n  {\"label\": \"sugar\", \"box_3d\": [0.19,0.77,0.3,0.3,0.12,0.25,44,-5,-2]},\n  {\"label\": \"sugar bowl\", \"box_3d\": [-0.11,0.71,0.13,0.18,0.13,0.18,-45,6,-15]},\n  {\"label\": \"dish towel\", \"box_3d\": [-0.27,0.6,-0.14,0.03,0.36,0.32,143,45,81]}\n]\n```";
                text = text.Replace("```json", "").Replace("```", "");
                Debug.Log(text);
                results = JsonConvert.DeserializeObject<Box3d[]>(text);
                return;
            }

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);
            model = client.GetModel(Models.Gemini_2_0_Flash);

            // Send request
            var blob = await inputTexture.ToJpgBlobAsync();

            Content[] messages = { new(Role.User, blob, inputText) };
            GenerateContentRequest request = new()
            {
                contents = messages,
                generationConfig = new GenerationConfig
                {
                    temperature = 0.5,
                },
            };
            var response = await model.GenerateContentAsync(request, destroyCancellationToken);
            if (response.candidates.Count == 0)
            {
                Debug.LogError("No response found");

            }
            var modelContent = response.candidates[0].content;
            bool success = TryGetJson(modelContent, out results);
            Debug.Log($"Success: {success}");
        }

        bool TryGetJson<T>(Content content, out T result)
        {
            if (content.parts.Count == 0)
            {
                result = default;
                return false;
            }
            var text = content.parts.First().text;
            if (string.IsNullOrWhiteSpace(text))
            {
                result = default;
                return false;
            }
            // Remove ```json and ``` from text
            text = text.Replace("```json", "").Replace("```", "");
            result = JsonConvert.DeserializeObject<T>(text);
            return result != null;
        }
    }
}
