#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoogleApis.GenerativeLanguage
{
    public enum Role
    {
        [EnumMember(Value = "user")]
        User,
        [EnumMember(Value = "model")]
        Model,
        [EnumMember(Value = "function")]
        Function,
    }

    /// <summary>
    /// The base structured datatype containing multi-part content of a message.
    /// A Content includes a role field designating the producer of the Content and a parts field containing multi-part data that contains the content of the message turn.
    /// https://ai.google.dev/api/rest/v1beta/Content
    /// </summary>
    public partial record Content
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Role? role;

        public ICollection<Part> parts;

        [JsonConstructor]
        public Content(Role role, params Part[] parts)
        {
            this.parts = parts;
            this.role = role;
        }

        public Content(Role role, Part part)
        {
            parts = new Part[] { part };
            this.role = role;
        }

        public Content(Role role, ICollection<Part> parts)
        {
            this.parts = parts;
            this.role = role;
        }

        public Content(Part[] parts)
        {
            this.parts = parts;
        }
    }

    // Internal types of Content
    public partial record Content
    {
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
            public string? text;
            /// <summary>
            /// Inline media bytes.
            /// </summary>
            public Blob? inlineData;
            /// <summary>
            /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
            /// </summary>
            public FunctionCall? functionCall;
            /// <summary>
            /// The result output of a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model.
            /// </summary>
            public FunctionResponse? functionResponse;
            /// <summary>
            /// URI based data.
            /// </summary>
            public FileData? fileData;

            public static implicit operator Part(string text) => new() { text = text };
            public static implicit operator Part(Blob inlineData) => new() { inlineData = inlineData };
            public static implicit operator Part(FunctionCall functionCall) => new() { functionCall = functionCall };
            public static implicit operator Part(FunctionResponse functionResponse) => new() { functionResponse = functionResponse };
            public static implicit operator Part(FileData fileData) => new() { fileData = fileData };
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

            [JsonConstructor]
            public Blob(string mimeType, string data)
            {
                this.mimeType = mimeType;
                this.data = data;
            }

            public Blob(string mimeType, ReadOnlySpan<byte> data)
            {
                this.mimeType = mimeType;
                this.data = Convert.ToBase64String(data);
            }
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
            public Dictionary<string, object>? args;

            public FunctionCall(string name, Dictionary<string, object>? args)
            {
                this.name = name;
                this.args = args;
            }
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
            public FunctionResponseContent response;

            [JsonConstructor]
            public FunctionResponse(string name, FunctionResponseContent response)
            {
                this.name = name;
                this.response = response;
            }

            public FunctionResponse(string name, object? content)
            {
                this.name = name;
                response = new FunctionResponseContent(name, content);
            }
        }

        // Undocumented redundant type used in FunctionResponse
        public record FunctionResponseContent
        {
            public string name;
            public object? content;

            public FunctionResponseContent(string name, object? content)
            {
                this.name = name;
                this.content = content;
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
            public string mimeType;
            /// <summary>
            /// Required. URI.
            /// </summary>
            public string fileUri;

            public FileData(string mimeType, string fileUri)
            {
                this.mimeType = mimeType;
                this.fileUri = fileUri;
            }
        }
    }
}
