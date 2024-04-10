#nullable enable

using Newtonsoft.Json;

// Other types
namespace GenerativeAI
{
    public partial record Tool
    {
        /// <summary>
        /// Optional. A list of FunctionDeclarations available to the model that can be used for function calling.
        /// 
        /// The model or system does not execute the function. Instead the defined function may be returned as a [FunctionCall][content.part.function_call] with arguments to the client side for execution. The model may decide to call a subset of these functions by populating [FunctionCall][content.part.function_call] in the response. The next conversation turn may contain a [FunctionResponse][content.part.function_response] with the [content.role] "function" generation context for the next model turn.
        /// </summary>
        public FunctionDeclaration[]? functionDeclarations;
    }

    partial record Tool
    {
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
    }
}
