#nullable enable

using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// A collection of source attributions for a piece of content.
    /// 
    /// https://ai.google.dev/api/rest/v1beta/CitationMetadata
    /// </summary>
    public partial record CitationMetadata
    {
        /// <summary>
        /// Citations to sources for a specific response.
        /// </summary>
        [JsonPropertyName("citationSources")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public CitationSource[]? CitationSources { get; set; }
    }

    /// <summary>
    /// A source attribution for a piece of content.
    /// </summary>
    public record CitationSource
    {
        /// <summary>
        /// Optional. Start of segment of the response that is attributed to this source.
        /// Index indicates the start of the segment, measured in bytes.
        /// </summary>
        [JsonPropertyName("startIndex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? StartIndex { get; set; }

        /// <summary>
        /// Optional. End of the attributed segment, exclusive.
        /// </summary>
        [JsonPropertyName("endIndex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? EndIndex { get; set; }

        /// <summary>
        /// Optional. URI that is attributed as a source for a portion of the text.
        /// </summary>
        [JsonPropertyName("uri")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Uri { get; set; }

        /// <summary>
        /// Optional. License for the GitHub project that is attributed as a source for segment.
        /// License info is required for code citations.
        /// </summary>
        [JsonPropertyName("license")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? License { get; set; }
    }
}
