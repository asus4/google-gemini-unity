#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/rest/v1beta/models/generateContent
    /// </summary>
    public record GenerateContentRequest
    {
        /// <summary>
        /// Required. The content of the current conversation with the model.
        ///
        /// For single-turn queries, this is a single instance. For multi-turn queries, this is a repeated field that contains conversation history + latest request.
        /// </summary>
        public ICollection<Content>? contents;

        /// <summary>
        /// Optional. A list of Tools the model may use to generate the next response.
        /// </summary>
        public ICollection<Tool>? tools;

        /// <summary>
        /// Optional. Tool configuration for any Tool specified in the request.
        /// </summary>
        public Tool.ToolConfig? toolConfig;

        /// <summary>
        /// Optional. A list of unique SafetySetting instances for blocking unsafe content.
        /// </summary>
        public ICollection<SafetySetting>? safetySettings;

        /// <summary>
        /// Optional. Developer set system instruction. Currently, text only.
        /// </summary>
        public Content? systemInstruction;

        /// <summary>
        /// Optional. Configuration options for model generation and outputs.
        /// </summary>
        public GenerationConfig? generationConfig;

        /// <summary>
        /// Optional. The name of the cached content used as context to serve the prediction. Note: only used in explicit caching, where users can have control over caching (e.g. what content to cache) and enjoy guaranteed cost savings. Format: cachedContents/{cachedContent}
        /// </summary>
        public string? cachedContent;

        // Constructor syntax sugars
        public static implicit operator GenerateContentRequest(Content[] contents) => new() { contents = contents };
        public static implicit operator GenerateContentRequest(List<Content> contents) => new() { contents = contents };
    }

    /// <summary>
    /// https://ai.google.dev/api/rest/v1beta/GenerateContentResponse
    /// </summary>
    public partial record GenerateContentResponse
    {
        public Candidate[] candidates;
        public PromptFeedback? promptFeedback;

        public GenerateContentResponse(Candidate[] candidates, PromptFeedback? promptFeedback)
        {
            this.candidates = candidates;
            this.promptFeedback = promptFeedback;
        }
        public override string ToString()
        {
            return this.SerializeToJson(true);
        }
    }

    partial record GenerateContentResponse
    {
        public enum BlockReason
        {
            BLOCK_REASON_UNSPECIFIED,
            SAFETY,
            OTHER,
        }

        public record PromptFeedback
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public BlockReason blockReason;
        }
    }
}
