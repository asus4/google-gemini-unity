#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Other types
namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// A response candidate generated from the model.
    /// 
    /// https://ai.google.dev/api/rest/v1beta/Candidate
    /// </summary>
    public partial record Candidate
    {
        public Content content;
        [JsonConverter(typeof(StringEnumConverter))]
        public FinishReason finishReason;
        public int index;
        public SafetyRating[]? safetyRatings;
        public CitationMetadata? citationMetadata;
        public int? tokenCount;
        public GroundingAttribution? groundingAttribution;

        public Candidate(Content content)
        {
            this.content = content;
        }
    }

    // Internal types of Candidate
    partial record Candidate
    {
        public enum FinishReason
        {
            [EnumMember(Value = "FINISH_REASON_UNSPECIFIED")]
            Unspecified, // Default value. This value is unused.
            [EnumMember(Value = "STOP")]
            Stop, // Natural stop point of the model or provided stop sequence.
            [EnumMember(Value = "MAX_TOKENS")]
            MaxTokens, // The maximum number of tokens as specified in the request was reached.
            [EnumMember(Value = "SAFETY")]
            Safety,// The candidate content was flagged for safety reasons.
            [EnumMember(Value = "RECITATION")]
            Recitation,// The candidate content was flagged for recitation reasons.
            [EnumMember(Value = "OTHER")]
            Other,
        }

        public record GroundingAttribution
        {
            public AttributionSourceId? sourceId;
            public Content? content;
        }

        public record AttributionSourceId
        {
            public GroundingPassageId? groundingPassage;
            public SemanticRetrieverChunk? semanticRetrieverChunk;
        }

        public record GroundingPassageId
        {
            public string? passageId;
            public int partIndex;
        }

        public record SemanticRetrieverChunk
        {
            public string? source;
            public string? chunk;
        }
    }
}
