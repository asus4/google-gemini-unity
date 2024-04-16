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
        public ICollection<Tool>? tools;

        /// <summary>
        /// In preview, subject to change.
        /// </summary>
        [JsonProperty("system_instruction")]
        public Content? systemInstruction;

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
