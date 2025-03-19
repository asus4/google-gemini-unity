#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        user,
        model,
        function,
    }

    /// <summary>
    /// The base structured datatype containing multi-part content of a message.
    /// A Content includes a role field designating the producer of the Content and a parts field containing multi-part data that contains the content of the message turn.
    /// https://ai.google.dev/api/caching#Content
    /// </summary>
    public partial record Content
    {
        /// <summary>
        /// Optional. The producer of the content. Must be either 'user' or 'model'.
        /// Useful to set for multi-turn conversations, otherwise can be left blank or unset.
        /// </summary>
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Role? Role { get; set; }

        /// <summary>
        /// Ordered Parts that constitute a single message. Parts may have different MIME types.
        /// </summary>
        [JsonPropertyName("parts")]
        public IReadOnlyList<Part> Parts { get; set; }


        [JsonConstructor]
        public Content(IReadOnlyList<Part> parts)
        {
            Parts = parts;
        }

        public Content(Role role, params Part[] parts)
        {
            Parts = parts;
            Role = role;
        }

        public Content(Role role, Part part)
        {
            Parts = new Part[] { part };
            Role = role;
        }

        public Content(Role role, IReadOnlyList<Part> parts)
        {
            Parts = parts;
            Role = role;
        }

        public Content(Part[] parts)
        {
            Parts = parts;
        }
    }

    // Internal types of Content
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
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Text { get; set; }

        /// <summary>
        /// Inline media bytes.
        /// </summary>
        [JsonPropertyName("inlineData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Blob? InlineData { get; set; }

        /// <summary>
        /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
        /// </summary>
        [JsonPropertyName("functionCall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FunctionCall? FunctionCall { get; set; }

        /// <summary>
        /// The result output of a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model.
        /// </summary>
        [JsonPropertyName("functionResponse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FunctionResponse? FunctionResponse { get; set; }

        /// <summary>
        /// URI based data.
        /// </summary>
        [JsonPropertyName("fileData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FileData? FileData { get; set; }

        // TODO:
        // public ExecutableCode executableCode
        // CodeExecutionResult codeExecutionResult

        public static implicit operator Part(string text) => new() { Text = text };
        public static implicit operator Part(Blob inlineData) => new() { InlineData = inlineData };
        public static implicit operator Part(FunctionCall functionCall) => new() { FunctionCall = functionCall };
        public static implicit operator Part(FunctionResponse functionResponse) => new() { FunctionResponse = functionResponse };
        public static implicit operator Part(FileData fileData) => new() { FileData = fileData };
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
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        /// <summary>
        /// Raw bytes for media formats. A base64-encoded string.
        /// </summary>
        [JsonPropertyName("data")]
        public ReadOnlyMemory<byte> Data { get; set; }

        [JsonConstructor]
        public Blob(string mimeType, ReadOnlyMemory<byte> data)
        {
            MimeType = mimeType;
            Data = data;
        }
    }

    /// <summary>
    /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
    /// </summary>
    public record FunctionCall
    {
        /// <summary>
        /// Optional. The unique id of the function call. If populated, the client to execute the functionCall and return the response with the matching id.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Id { get; set; }

        /// <summary>
        /// Required. The name of the function to call. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Optional. The function parameters and values in JSON object format.
        /// </summary>
        [JsonPropertyName("args")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Dictionary<string, object>? Args { get; set; }

        public FunctionCall(string name, Dictionary<string, object>? args)
        {
            Name = name;
            Args = args;
        }
    }

    /// <summary>
    /// The result output from a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model. This should contain the result of aFunctionCall made based on model prediction.
    /// </summary>
    public record FunctionResponse
    {
        /// <summary>
        /// Optional. The id of the function call this response is for. Populated by the client to match the corresponding function call id.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Id { get; set; }

        /// <summary>
        /// Required. The name of the function to call. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 63.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Required. The function response in JSON object format.
        /// </summary>
        [JsonPropertyName("response")]
        public FunctionResponseContent Response { get; set; }

        [JsonConstructor]
        public FunctionResponse(string name, FunctionResponseContent response)
        {
            Name = name;
            Response = response;
        }

        public FunctionResponse(string name, object? content)
        {
            Name = name;
            Response = new FunctionResponseContent(name, content);
        }
    }

    // Undocumented redundant type used in FunctionResponse
    public record FunctionResponseContent
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("content")]
        public object? Content { get; set; }

        public FunctionResponseContent(string name, object? content)
        {
            Name = name;
            Content = content;
        }
    }

    /// <summary>
    /// URI based data.
    /// </summary>
    public record FileData
    {
        /// <summary>
        /// Required. The IANA standard MIME type of the source data.
        /// </summary>
        [JsonPropertyName("mimeType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MimeType { get; set; }

        /// <summary>
        /// Required. URI.
        /// </summary>
        [JsonPropertyName("fileUri")]
        public string FileUri { get; set; }

        public FileData(string mimeType, string fileUri)
        {
            MimeType = mimeType;
            FileUri = fileUri;
        }
    }
}
