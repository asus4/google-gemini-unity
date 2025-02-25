using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoogleApis.GenerativeLanguage;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

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
        class BoundingBox3d
        {
            /// <summary>
            /// A label of the object.
            /// </summary>
            [JsonPropertyName("label")]
            public string Label { get; set; }

            /// <summary>
            /// x_center, y_center, z_center, x_size, y_size, z_size, roll, pitch, yaw
            /// </summary>
            [JsonPropertyName("box_3d")]
            public float[] Values { get; set; }

            public float3 Position => new(Values[0], Values[1], Values[2]);
            public float3 Size => new(Values[3], Values[4], Values[5]);
            public float3 EulerAngles => new(Values[6], Values[7], Values[8]);
            public quaternion Rotation => quaternion.Euler(EulerAngles);
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
        private float3 eulerOffset;

        [SerializeField]
        BoundingBox3d[] results;

        private GenerativeModel model;
        private readonly Vector3[] worldCorners = new Vector3[4];




        private async void Start()
        {
            // Set image
            rawImage.texture = inputTexture;
            if (rawImage.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
            {
                aspectRatioFitter.aspectRatio = (float)inputTexture.width / inputTexture.height;
            }

            // FIXME: skipping API call for quick testing
            if (isTest)
            {
#pragma warning disable JSON001 // Invalid JSON pattern
                string text = "```json\n[\n  {\"label\": \"sugar container\", \"box_3d\": [0.22,1.16,0.46,0.46,0.36,0.46,-34,0,-2]},\n  {\"label\": \"white ceramic container\", \"box_3d\": [-0.12,0.98,0.13,0.21,0.17,0.27,-58,-48,48]},\n  {\"label\": \"brown liquid\", \"box_3d\": [0.24,0.93,-0.04,0.04,0.18,0.32,133,34,87]},\n  {\"label\": \"brown and white napkin\", \"box_3d\": [-0.31,0.83,-0.16,0.05,0.4,0.42,145,34,87]}\n]\n```";
                text = text.Replace("```json", "").Replace("```", "");
#pragma warning restore JSON001 // Invalid JSON pattern
                Debug.Log(text);
                results = JsonSerializer.Deserialize<BoundingBox3d[]>(text);
                return;
            }

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // Gemini 2.0 Pro returns better results
            // model = client.GetModel(Models.Gemini_2_0_Flash);
            model = client.GetModel(Models.Gemini_2_0_Pro_Exp);

            // Send request
            var blob = await inputTexture.ToJpgBlobAsync();

            Content[] messages = { new(Role.user, blob, inputText) };
            GenerateContentRequest request = new()
            {
                Contents = messages,
                GenerationConfig = new()
                {
                    Temperature = 0.5,
                },
            };
            var response = await model.GenerateContentAsync(request, destroyCancellationToken);
            if (response.Candidates.Length == 0)
            {
                Debug.LogError("No response found");

            }
            var modelContent = response.Candidates[0].Content;
            bool success = TryDeserializeJson(modelContent, out results);
            Debug.Log($"Success: {success}");
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (results.Length == 0)
            {
                return;
            }

            var rt = rawImage.rectTransform;
            rt.GetWorldCorners(worldCorners);
            float3 rtPosition = rt.position;
            float3 rectSize = worldCorners[2] - worldCorners[0];

            Gizmos.color = Color.green;
            UnityEditor.Handles.color = Color.green;

            foreach (var result in results)
            {
                Gizmos.matrix = Matrix4x4.TRS(
                    (result.Position + new float3(0, -1, 0)) * new float3(rectSize.x, rectSize.y, 1) + rtPosition,
                    quaternion.EulerZYX((result.EulerAngles + eulerOffset) * Mathf.Deg2Rad),
                    result.Size * new float3(rectSize.y)
                );
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

                // Draw handle
                float3 center = (result.Position + new float3(0, -1, 0)) * new float3(rectSize.x, rectSize.y, 1) + rtPosition;
                UnityEditor.Handles.Label(center, result.Label);
            }

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawSphere(rtPosition, 0.1f);
        }
#endif // UNITY_EDITOR

        static float3x3 GetIntrinsics(float2 size, float fov)
        {
            float f = size.x / (2 * math.tan(fov / 2f * math.PI / 180));
            float cx = size.x / 2;
            float cy = size.y / 2;
            return new(
                f, 0, cx,
                0, f, cy,
                0, 0, 1
            );
        }

        const float tiltAngle = 90 * math.PI / 180;
        readonly float3x3 viewRotationMatrix = new(
            1, 0, 0,
            0, math.cos(tiltAngle), -math.sin(tiltAngle),
            0, math.sin(tiltAngle), math.cos(tiltAngle)
        );

        static bool TryDeserializeJson<T>(Content content, out T result)
        {
            if (content.Parts.Count == 0)
            {
                result = default;
                return false;
            }
            var text = content.Parts.First().Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                result = default;
                return false;
            }
            // Remove ```json and ``` from text
            text = text.Replace("```json", "").Replace("```", "");
            result = JsonSerializer.Deserialize<T>(text);
            return result != null;
        }
    }
}
