#nullable enable

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    public partial record GenerateImageRequest
    {
        public Instance[] instances;
        public Parameters parameters;

        public GenerateImageRequest(string prompt, Parameters parameters)
        {
            instances = new Instance[] { new(prompt) };
            this.parameters = parameters;
        }
    }

    public partial record GenerateImageRequest
    {
        public record Instance
        {
            public string prompt;

            public Instance(string prompt)
            {
                this.prompt = prompt;
            }
        }

        public enum AspectRatio
        {
            [EnumMember(Value = "1:1")]
            [InspectorName("1:1")]
            Ratio1by1,
            [EnumMember(Value = "4:3")]
            [InspectorName("4:3")]
            Ratio4by3,
            [EnumMember(Value = "3:4")]
            [InspectorName("3:4")]
            Ratio3by4,
            [EnumMember(Value = "16:9")]
            [InspectorName("16:9")]
            Ratio16by9,
            [EnumMember(Value = "9:16")]
            [InspectorName("9:16")]
            Ratio9by16,
        }

        public enum PersonGeneration
        {
            [EnumMember(Value = "dont_allow")]
            DontAllow,
            [EnumMember(Value = "allow_adult")]
            AllowAdult,
        }

        public struct Parameters
        {
            public int sampleCount;
            [JsonConverter(typeof(StringEnumConverter))]
            public AspectRatio aspectRatio;
            [JsonConverter(typeof(StringEnumConverter))]
            public PersonGeneration personGeneration;
        }

    }

    public partial record GenerateImageResponse
    {
        public Prediction[] predictions;

        public GenerateImageResponse(Prediction[] predictions)
        {
            this.predictions = predictions;
        }

        public override string ToString()
        {
            return this.SerializeToJson(true);
        }
    }

    public partial record GenerateImageResponse
    {
        public record Prediction
        {
            public string mimeType;
            public string bytesBase64Encoded;

            public Prediction(string mimeType, string bytesBase64Encoded)
            {
                this.mimeType = mimeType;
                this.bytesBase64Encoded = bytesBase64Encoded;
            }

            public Texture2D ToTexture(bool markNonReadable = false)
            {
                var texture = new Texture2D(1, 1);
                var data = Convert.FromBase64String(bytesBase64Encoded);
                texture.LoadImage(data, markNonReadable);
                texture.Apply();
                return texture;
            }
        }
    }
}
