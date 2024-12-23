#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Tool details that the model may use to generate response.
    /// 
    /// A Tool is a piece of code that enables the system to interact with external systems to perform an action, or set of actions, outside of knowledge and scope of the model.
    /// https://ai.google.dev/api/caching#Tool
    /// </summary>
    public partial record Tool
    {
        /// <summary>
        /// Optional. A list of FunctionDeclarations available to the model that can be used for function calling.
        /// 
        /// The model or system does not execute the function. Instead the defined function may be returned as a [FunctionCall][content.part.function_call] with arguments to the client side for execution. The model may decide to call a subset of these functions by populating [FunctionCall][content.part.function_call] in the response. The next conversation turn may contain a [FunctionResponse][content.part.function_response] with the [content.role] "function" generation context for the next model turn.
        /// </summary>
        public FunctionDeclaration[]? functionDeclarations;

        /// <summary>
        /// Optional. Retrieval tool that is powered by Google search.
        /// NOTE: The online document is `googleSearchRetrieval`. But it was 400 error.
        /// </summary>
        public GoogleSearchRetrieval? googleSearch;

        public override string ToString()
            => this.SerializeToJson(true);

        public static implicit operator Tool(FunctionDeclaration[] functionDeclarations)
            => new() { functionDeclarations = functionDeclarations };
        public static implicit operator Tool(GoogleSearchRetrieval googleSearchRetrieval)
            => new() { googleSearch = googleSearchRetrieval };
    }

    partial record Tool
    {
        /// <summary>
        /// Structured representation of a function declaration as defined by the OpenAPI 3.03 specification.
        /// Included in this declaration are the function name and parameters. This FunctionDeclaration is a representation of a block of code that can be used as a Tool by the model and executed by the client.
        /// 
        /// https://spec.openapis.org/oas/v3.0.3
        /// </summary>
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

            /// <summary>
            /// Optional. Describes the parameters to this function. Reflects the Open API 3.03 Parameter Object string Key: the name of the parameter. Parameter names are case sensitive. Schema Value: the Schema defining the type used for the parameter.
            /// </summary>
            public Schema? parameters;

            public FunctionDeclaration(string name, string description, Schema? parameters)
            {
                this.name = name;
                this.description = description;
                this.parameters = parameters;
            }
        }

        public record Schema
        {
            /// <summary>
            /// Data type
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Type type;

            /// <summary>
            /// Optional. The format of the data. This is used only for primitive datatypes. Supported formats: for NUMBER type: float, double for INTEGER type: int32, int64
            /// </summary>
            public string? format;

            /// <summary>
            /// Optional. A brief description of the parameter. This could contain examples of use. Parameter description may be formatted as Markdown.
            /// </summary>
            public string? description;

            /// <summary>
            /// Optional. Indicates if the value may be null.
            /// </summary>
            public bool? nullable;

            /// <summary>
            /// Optional. Possible values of the element of Type.STRING with enum format. For example we can define an Enum Direction as : {type:STRING, format:enum, enum:["EAST", NORTH", "SOUTH", "WEST"]}
            /// </summary>
            [JsonProperty("enum")]
            public string[]? enums;

            /// <summary>
            /// Optional. Properties of Type.OBJECT.
            /// </summary>
            public Dictionary<string, Schema>? properties;

            /// <summary>
            /// Optional. Required properties of Type.OBJECT.
            /// </summary>
            public string[]? required;

            /// <summary>
            /// Optional. Schema of the elements of Type.ARRAY.
            /// </summary>
            public Schema? items;

            public override string ToString()
            {
                return this.SerializeToJson(true);
            }
        }

        /// <summary>
        /// Type contains the list of OpenAPI data types as defined by https://spec.openapis.org/oas/v3.0.3#data-types
        /// </summary>
        public enum Type
        {
            TYPE_UNSPECIFIED,
            STRING,
            NUMBER,
            INTEGER,
            BOOLEAN,
            ARRAY,
            OBJECT,
        }

        /// <summary>
        /// The Tool configuration containing parameters for specifying Tool use in the request.
        /// https://ai.google.dev/api/rest/v1beta/cachedContents#toolconfig
        /// </summary>
        public record ToolConfig
        {
            public FunctionCallingConfig? functionCallingConfig;
        }

        /// <summary>
        /// Configuration for specifying function calling behavior.
        /// https://ai.google.dev/api/rest/v1beta/cachedContents#toolconfig
        /// </summary>
        public record FunctionCallingConfig
        {
            public enum Mode
            {
                // Unspecified function calling mode. This value should not be used.
                MODE_UNSPECIFIED,
                // Default model behavior, model decides to predict either a function call or a natural language response.
                AUTO,
                // Model is constrained to always predicting a function call only. If "allowedFunctionNames" are set, the predicted function call will be limited to any one of "allowedFunctionNames", else the predicted function call will be any one of the provided "functionDeclarations".
                ANY,
                // Model will not predict any function call. Model behavior is same as when not passing any function declarations.
                NONE,
            }

            /*
            JSON representation

            {
            "mode": enum (Mode),
            "allowedFunctionNames": [
                string
            ]
            }
            */

            [JsonConverter(typeof(StringEnumConverter))]
            public Mode? mode;
            public string[]? allowedFunctionNames;
        }

        // https://ai.google.dev/api/caching#GoogleSearchRetrieval
        public record GoogleSearchRetrieval
        {
            public DynamicRetrievalConfig? dynamicRetrievalConfig;
        }

        public record DynamicRetrievalConfig
        {
            public enum Mode
            {
                // Always trigger retrieval.
                MODE_UNSPECIFIED,
                // Run retrieval only when system decides it is necessary.
                MODE_DYNAMIC,
            }

            public Mode? mode;
            public double? dynamicThreshold;
        }
    }
}
