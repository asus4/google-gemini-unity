#nullable enable

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Other types
namespace GenerativeAI
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
        public Content[] contents;
        public Tool[]? tools;

        public GenerateContentRequest(string text)
        {
            contents = new Content[]
            {
                new()
                {
                    parts = new Content.Part[]
                    {
                        new() { text = text, },
                    },
                },
            };
        }
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
            return this.SerializeToJson();
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
