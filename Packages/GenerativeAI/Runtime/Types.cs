#nullable enable

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenerativeAI
{
    /// <summary>
    /// Call API to know all available models for your API Key.
    /// https://generativelanguage.googleapis.com/v1beta/models?key={YOUR_API_KEY}
    /// </summary>
    public static class Models
    {
        public const string GeminiPro = "models/gemini-pro";
        public const string GeminiProVision = "models/gemini-pro-vision";

        public const string Gemini_1_5_Pro = "models/gemini-1.5-pro-latest";
        public const string Gemini_1_5_ProVision = "models/gemini-1.5-pro-vision-latest";
    }

    public record FunctionDeclaration
    {
        /// <summary>
        /// Required. The name of the function to call. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        public string name;

        /// <summary>
        /// Required. A brief description of the function.
        /// </summary>
        public string description;

        [JsonProperty(PropertyName = "object")]
        public object? Object { get; set; }

        public FunctionDeclaration(string name, string description, object? Object = null)
        {
            this.name = name;
            this.description = description;
            this.Object = Object;
        }
    }

    public record Tool
    {
        /// <summary>
        /// Optional. A list of FunctionDeclarations available to the model that can be used for function calling.
        /// 
        /// The model or system does not execute the function. Instead the defined function may be returned as a [FunctionCall][content.part.function_call] with arguments to the client side for execution. The model may decide to call a subset of these functions by populating [FunctionCall][content.part.function_call] in the response. The next conversation turn may contain a [FunctionResponse][content.part.function_response] with the [content.role] "function" generation context for the next model turn.
        /// </summary>
        public FunctionDeclaration[]? functionDeclarations;
    }

    /// <summary>
    /// https://ai.google.dev/api/rest/v1beta/GenerateContentResponse
    /// </summary>
    public record GenerateContentResponse
    {
        public Content[]? contents;
    }

    internal static class JsonExtensions
    {
        private static readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static string ToJson(this GenerateContentRequest requestBody)
            => JsonConvert.SerializeObject(requestBody, settings);
    }
}
