#nullable enable

using System;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    public partial record GenerateImageRequest
    {
        [JsonPropertyName("instances")]
        public Instance[] Instances { get; set; }

        [JsonPropertyName("parameters")]
        public ImageGenerationParameters Parameters { get; set; }

        [JsonConstructor]
        public GenerateImageRequest(Instance[] instances, ImageGenerationParameters parameters)
        {
            Instances = instances;
            Parameters = parameters;
        }

        public GenerateImageRequest(string prompt, ImageGenerationParameters parameters)
        {
            Instances = new Instance[] { new(prompt) };
            Parameters = parameters;
        }
    }

    public partial record GenerateImageRequest
    {
        public record Instance
        {
            [JsonPropertyName("prompt")]
            public string Prompt { get; set; }

            public Instance(string prompt)
            {
                Prompt = prompt;
            }
        }

        public enum AspectRatioEnum
        {
            [InspectorName("1:1")]
            Ratio1by1,
            [InspectorName("4:3")]
            Ratio4by3,
            [InspectorName("3:4")]
            Ratio3by4,
            [InspectorName("16:9")]
            Ratio16by9,
            [InspectorName("9:16")]
            Ratio9by16,
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PersonGeneration
        {
            dont_allow,
            allow_adult,
        }

        public struct ImageGenerationParameters
        {
            [JsonPropertyName("sampleCount")]
            public int SampleCount { get; set; }

            [JsonPropertyName("aspectRatio")]
            public string AspectRatio { get; set; }

            [JsonPropertyName("personGeneration")]
            public PersonGeneration PersonGeneration { get; set; }

            [JsonIgnore]
            public AspectRatioEnum AspectRatioEnum
            {
                readonly get => AspectRatio switch
                {
                    "1:1" => AspectRatioEnum.Ratio1by1,
                    "4:3" => AspectRatioEnum.Ratio4by3,
                    "3:4" => AspectRatioEnum.Ratio3by4,
                    "16:9" => AspectRatioEnum.Ratio16by9,
                    "9:16" => AspectRatioEnum.Ratio9by16,
                    _ => throw new NotSupportedException($"Unsupported aspect ratio: {AspectRatio}")
                };
                set => AspectRatio = value switch
                {
                    AspectRatioEnum.Ratio1by1 => "1:1",
                    AspectRatioEnum.Ratio4by3 => "4:3",
                    AspectRatioEnum.Ratio3by4 => "3:4",
                    AspectRatioEnum.Ratio16by9 => "16:9",
                    AspectRatioEnum.Ratio9by16 => "9:16",
                    _ => throw new NotSupportedException($"Unsupported aspect ratio: {value}")
                };
            }
        }
    }

    public partial record GenerateImageResponse
    {
        [JsonPropertyName("predictions")]
        public Prediction[] Predictions { get; set; }

        public GenerateImageResponse(Prediction[] predictions)
        {
            Predictions = predictions;
        }

        public override string ToString() => this.SerializeToJson(true);
    }

    public partial record GenerateImageResponse
    {
        public record Prediction
        {
            [JsonPropertyName("mimeType")]
            public string MimeType { get; set; }

            [JsonPropertyName("bytesBase64Encoded")]
            public string BytesBase64Encoded { get; set; }

            public Prediction(string mimeType, string bytesBase64Encoded)
            {
                MimeType = mimeType;
                BytesBase64Encoded = bytesBase64Encoded;
            }

            public Texture2D ToTexture(bool markNonReadable = false)
            {
                var texture = new Texture2D(1, 1);
                var data = Convert.FromBase64String(BytesBase64Encoded);
                texture.LoadImage(data, markNonReadable);
                texture.Apply();
                return texture;
            }
        }
    }
}
