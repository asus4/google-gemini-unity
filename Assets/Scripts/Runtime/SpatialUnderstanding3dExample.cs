// Original source:
// https://github.com/google-gemini/cookbook/blob/main/examples/Spatial_understanding_3d.ipynb
// Ported to Unity3D by @asus4
//
// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GoogleApis.GenerativeLanguage;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Example of Spatial Understanding 3D (experimental).
    /// 
    /// Original source:
    /// https://github.com/google-gemini/cookbook/blob/main/examples/Spatial_understanding_3d.ipynb
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
            [field: SerializeField]
            public string Label { get; set; }

            /// <summary>
            /// x_center, y_center, z_center, x_size, y_size, z_size, roll, pitch, yaw
            /// </summary>
            [JsonPropertyName("box_3d")]
            [field: SerializeField]
            public float[] Values { get; set; }

            public Vector3 Position => new(Values[0], Values[1], Values[2]);
            public Vector3 Size => new(Values[3], Values[4], Values[5]);
            // public Vector3 EulerAngles => new(Values[6], Values[7], Values[8]);
            // public Quaternion Rotation => Quaternion.Euler(EulerAngles);

            public Quaternion GetRotation(math.RotationOrder rotationOrder)
            {
                var euler = new float3(Values[6], Values[7], Values[8]);
                // return Quaternion.Euler(euler);
                return quaternion.Euler(math.radians(euler), rotationOrder);
            }
        }

        [SerializeField]
        Texture inputTexture;

        [SerializeField]
        bool useTest = true;

        [SerializeField]
        TextAsset testData;

        [SerializeField]
        RawImage rawImage;

        [SerializeField]
        [TextArea]
        string inputText = "Output in json. Detect the 3D bounding boxes of items , output no more than 10 items. Return a list where each entry contains the object name in \"label\" and its 3D bounding box in \"box_3d\".";

        [SerializeField]
        [Range(10f, 120f)]
        float fieldOfView = 69f;

        [SerializeField]
        [Range(0.001f, 0.02f)]
        float scale = 0.005f;

        [SerializeField]
        math.RotationOrder rotationOrder = math.RotationOrder.XYZ;

        [SerializeField]
        BoundingBox3d[] results;

        GenerativeModel model;

        // Gemini -> Unity Camera conversion matrix
        static readonly Matrix4x4 GeminiToUnityCameraMatrix
            // Flip Y for Unity coordinate system
            = Matrix4x4.Scale(new(1, -1, 1))
            // Rotate 90 degrees around X
            * Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));

        static readonly Vector3[] UnitCorners = new Vector3[]
         {
            new(0.5f, -0.5f, -0.5f), // 1
            new(0.5f, 0.5f, -0.5f), // 3
            new(0.5f, 0.5f, 0.5f),  // 7
            new(0.5f, -0.5f, 0.5f),  // 5
            new(-0.5f, -0.5f, -0.5f), // 0
            new(-0.5f, 0.5f, -0.5f), // 2
            new(-0.5f, 0.5f, 0.5f),  // 6
            new(-0.5f, -0.5f, 0.5f), // 4
            Vector3.zero // Center
         };

        async void Start()
        {
            // Set image
            rawImage.texture = inputTexture;
            if (rawImage.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
            {
                aspectRatioFitter.aspectRatio = (float)inputTexture.width / inputTexture.height;
            }

            // FIXME: skipping API call for quick testing
            if (useTest && testData != null && TryDeserializeJson(testData.text, out results))
            {
                return;
            }

            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // Gemini 2.0 Pro returns better results
            model = client.GetModel(Models.Gemini_2_0_Flash);
            // model = client.GetModel(Models.Gemini_2_0_Pro_Exp);

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

            if (success)
            {
                Debug.Log(modelContent.Parts.First().Text);
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (inputTexture != null && rawImage != null)
            {
                rawImage.texture = inputTexture;
                if (rawImage.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
                {
                    aspectRatioFitter.aspectRatio = (float)inputTexture.width / inputTexture.height;
                }
            }

            if (useTest && testData != null)
            {
                TryDeserializeJson(testData.text, out results);
            }
        }

        void OnDrawGizmos()
        {
            if (results.Length == 0)
            {
                return;
            }

            var rt = rawImage.rectTransform;
            // Debug.Log($"rt size: {rt.sizeDelta} rect: {rt.rect} position: {rt.position}");

            float aspectRatio = rt.rect.width / rt.rect.height;
            float scale = this.scale;
            Vector2 worldScale = new(scale / aspectRatio, scale);
            Vector3 worldCenter = rt.position;

            float focalLength = (worldScale.x * rt.rect.width) / (2 * Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad));


            Span<Vector3> corners = stackalloc Vector3[UnitCorners.Length];
            Span<Vector3> projectedPoints = stackalloc Vector3[UnitCorners.Length];

            foreach (var result in results)
            {
                // Rest corners
                UnitCorners.CopyTo(corners);

                // Apply object -> view matrix
                Matrix4x4 localToCameraMatrix = GeminiToUnityCameraMatrix
                    * Matrix4x4.TRS(
                        result.Position,
                        result.GetRotation(rotationOrder),
                        result.Size
                    );
                for (int i = 0; i < corners.Length; i++)
                {
                    corners[i] = localToCameraMatrix.MultiplyPoint(corners[i]);
                }

                // Draw corners
                {
                    Gizmos.color = Color.blue;
                    DrawWireBox(corners[..^1]);
                }
                // Draw wire cube
                {
                    Gizmos.color = Color.red;
                    Gizmos.matrix = localToCameraMatrix;
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    UnityEditor.Handles.Label(corners[^1], result.Label);
                }
                Gizmos.matrix = Matrix4x4.identity;

                // Apply projection matrix
                for (int i = 0; i < corners.Length; i++)
                {
                    Vector2 projected = focalLength * new Vector2(
                        corners[i].x / corners[i].z,
                        corners[i].y / corners[i].z
                    );
                    projected += worldScale;
                    projectedPoints[i] = (Vector3)projected + worldCenter;
                }

                Gizmos.color = Color.green;
                DrawWireBox(projectedPoints[..^1]);
                UnityEditor.Handles.Label(projectedPoints[^1], result.Label);
            }

            Gizmos.matrix = Matrix4x4.identity;
        }

        static void DrawWireBox(Span<Vector3> vertices)
        {
            // Split vertices into top and bottom
            var topVertices = vertices[..4];
            var bottomVertices = vertices[4..];

            for (int i = 0; i < 4; i++)
            {
                // Top lines
                Gizmos.DrawLine(topVertices[i], topVertices[(i + 1) % 4]);
                // Bottom lines
                Gizmos.DrawLine(bottomVertices[i], bottomVertices[(i + 1) % 4]);
                // Connecting lines
                Gizmos.DrawLine(topVertices[i], bottomVertices[i]);
            }
        }
#endif // UNITY_EDITOR

        static bool TryDeserializeJson<T>(Content content, out T result)
        {
            if (content.Parts.Count == 0)
            {
                result = default;
                return false;
            }
            var text = content.Parts.First().Text;
            return TryDeserializeJson(text, out result);
        }

        static bool TryDeserializeJson<T>(string text, out T result)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                result = default;
                return false;
            }
            // Remove ```json and ``` from text
            text = text
            .Replace("```json", "")
                .Replace("```", "");
            result = JsonSerializer.Deserialize<T>(text);
            return result != null;
        }
    }
}
