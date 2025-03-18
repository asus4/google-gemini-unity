#nullable enable

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Generates a model response given an input GenerateContentRequest.
    /// https://ai.google.dev/api/generate-content#v1beta.models.generateContent
    /// </summary>
    public class GenerateContentRequest
    {
        /// <summary>
        /// Required. The content of the current conversation with the model.
        ///
        /// For single-turn queries, this is a single instance. For multi-turn queries, this is a repeated field that contains conversation history + latest request.
        /// </summary>
        [JsonPropertyName("contents")]
        public ICollection<Content>? Contents { get; set; }

        /// <summary>
        /// Optional. A list of Tools the model may use to generate the next response.
        /// </summary>
        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<Tool>? Tools { get; set; }

        /// <summary>
        /// Optional. Tool configuration for any Tool specified in the request.
        /// </summary>
        [JsonPropertyName("toolConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Tool.ToolConfig? ToolConfig { get; set; }

        /// <summary>
        /// Optional. A list of unique SafetySetting instances for blocking unsafe content.
        /// </summary>
        [JsonPropertyName("safetySettings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<SafetySetting>? SafetySettings { get; set; }

        /// <summary>
        /// Optional. Developer set system instruction. Currently, text only.
        /// </summary>
        [JsonPropertyName("systemInstruction")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Content? SystemInstruction { get; set; }

        /// <summary>
        /// Optional. Configuration options for model generation and outputs.
        /// </summary>
        [JsonPropertyName("generationConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public GenerationConfig? GenerationConfig { get; set; }

        /// <summary>
        /// Optional. The name of the cached content used as context to serve the prediction. Note: only used in explicit caching, where users can have control over caching (e.g. what content to cache) and enjoy guaranteed cost savings. Format: cachedContents/{cachedContent}
        /// </summary>
        [JsonPropertyName("cachedContent")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? CachedContent { get; set; }

        // Constructor syntax sugars
        public static implicit operator GenerateContentRequest(Content[] contents) => new() { Contents = contents };
        public static implicit operator GenerateContentRequest(List<Content> contents) => new() { Contents = contents };
    }

    /// <summary>
    /// https://ai.google.dev/api/generate-content#v1beta.GenerateContentResponse
    /// </summary>
    public class GenerateContentResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[] Candidates { get; set; }

        [JsonPropertyName("promptFeedback")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PromptFeedback? PromptFeedback { get; set; }

        // TODO:
        // usageMetadata
        // modelVersion

        [JsonConstructor]
        public GenerateContentResponse(Candidate[] candidates)
        {
            Candidates = candidates;
        }

        public override string ToString()
        {
            return this.SerializeToJson(true);
        }
    }

    /// <summary>
    /// A set of the feedback metadata the prompt specified in GenerateContentRequest.content.
    /// </summary>
    public record PromptFeedback
    {
        [JsonPropertyName("blockReason")]
        public BlockReason BlockReason { get; set; }

        // TODO:
        // safetyRatings
    }

    /// <summary>
    /// Specifies the reason why the prompt was blocked.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BlockReason
    {
        BLOCK_REASON_UNSPECIFIED,
        SAFETY,
        OTHER,
        BLOCKLIST,
        PROHIBITED_CONTENT,
        IMAGE_SAFETY,
    }
}
