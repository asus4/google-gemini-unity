using System;
// using System.Text.Json.Serialization;
// using System.Text.Json;

namespace GenerativeAI
{
    [Serializable]
    public class Content
    {
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
        [Serializable]
        public sealed class Part
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
        [Serializable]
        public sealed class Blob
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
        [Serializable]
        public sealed class FunctionCall
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
        [Serializable]
        public sealed class FunctionResponse
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
        [Serializable]
        public sealed class FileData
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

        public Role role;
    }

    [Serializable]
    public record Tool
    {
        [Serializable]
        public sealed class FunctionDeclaration
        {

        }
    }

    [Serializable]
    public sealed class RequestBody
    {
        /// <summary>
        /// Required. The content of the current conversation with the model.
        /// 
        /// For single-turn queries, this is a single instance. For multi-turn queries, this is a repeated field that contains conversation history + latest request.
        /// </summary>
        public Content[] content;
        public Tool[] tools;
    }
}
