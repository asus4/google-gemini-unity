using System;
using System.Text;
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
    }

    public enum Role
    {
        user,
        model,
    }

    /// <summary>
    /// A datatype containing media that is part of a multi-part Content message.
    /// A Part consists of data which has an associated datatype.A Part can only contain one of the accepted types in Part.data.
    /// A Part must have a fixed IANA MIME type identifying the type and subtype of the media if the inlineData field is filled with raw bytes.
    /// </summary>
    public record Part
    {
        /// <summary>
        /// Inline text.
        /// </summary>
        public string text;
        /// <summary>
        /// Inline media bytes.
        /// </summary>
        public Blob inlineData;
        /// <summary>
        /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
        /// </summary>
        public FunctionCall functionCall;
        /// <summary>
        /// The result output of a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model.
        /// </summary>
        public FunctionResponse functionResponse;
        /// <summary>
        /// URI based data.
        /// </summary>
        public FileData fileData;
    }

    /// <summary>
    /// Raw media bytes.
    /// Text should not be sent as raw bytes, use the 'text' field.
    /// </summary>
    public record Blob
    {
        /// <summary>
        /// The IANA standard MIME type of the source data. Accepted types include: "image/png", "image/jpeg", "image/heic", "image/heif", "image/webp".
        /// </summary>
        public string mimeType;
        /// <summary>
        /// Raw bytes for media formats. A base64-encoded string.
        /// </summary>
        public string data;
    }

    /// <summary>
    /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
    /// </summary>
    public record FunctionCall
    {
        /// <summary>
        /// Required. The name of the function to call. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        public string name;

        /// <summary>
        /// Optional. The function parameters and values in JSON object format.
        /// </summary>
        // public string args;
    }

    /// <summary>
    /// The result output from a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model. This should contain the result of aFunctionCall made based on model prediction.
    /// </summary>
    public record FunctionResponse
    {
        /// <summary>
        /// Required. The name of the function to call. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        public string name;

        /// <summary>
        /// Required. The function response in JSON object format.
        /// </summary>
        public string response;
    }

    /// <summary>
    /// URI based data.
    /// </summary>
    public record FileData
    {
        /// <summary>
        /// Required. The IANA standard MIME type of the source data.
        /// </summary>
        public string mimeType;
        /// <summary>
        /// Required. URI.
        /// </summary>
        public string fileUri;
    }

    /// <summary>
    /// The base structured datatype containing multi-part content of a message.
    ///
    /// A Content includes a role field designating the producer of the Content and a parts field containing multi-part data that contains the content of the message turn.
    /// </summary>
    public record Content
    {
        public Part[] parts;

        [JsonConverter(typeof(StringEnumConverter))]
        public Role? role;
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
        public object Object { get; set; }
    }

    public record Tool
    {
        /// <summary>
        /// Optional. A list of FunctionDeclarations available to the model that can be used for function calling.
        /// 
        /// The model or system does not execute the function. Instead the defined function may be returned as a [FunctionCall][content.part.function_call] with arguments to the client side for execution. The model may decide to call a subset of these functions by populating [FunctionCall][content.part.function_call] in the response. The next conversation turn may contain a [FunctionResponse][content.part.function_response] with the [content.role] "function" generation context for the next model turn.
        /// </summary>
        public FunctionDeclaration[] functionDeclarations;
    }

    public record RequestBody
    {
        /// <summary>
        /// Required. The content of the current conversation with the model.
        /// 
        /// For single-turn queries, this is a single instance. For multi-turn queries, this is a repeated field that contains conversation history + latest request.
        /// </summary>
        [JsonProperty(PropertyName = "contents")]
        public Content[] Contents { get; set; }

        [JsonProperty(PropertyName = "tools")]
        public Tool[] Tools { get; set; }

        public RequestBody(string text)
        {
            Contents = new Content[]
            {
                new()
                {
                    parts = new Part[]
                    {
                        new() { text = text, },
                    },
                },
            };
        }
    }

    internal static class JsonExtensions
    {
        private static readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static string ToJson(this RequestBody requestBody)
            => JsonConvert.SerializeObject(requestBody, settings);
    }
}
