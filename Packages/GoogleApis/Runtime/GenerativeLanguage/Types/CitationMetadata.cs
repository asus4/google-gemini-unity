#nullable enable

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// A collection of source attributions for a piece of content.
    /// 
    /// https://ai.google.dev/api/rest/v1beta/CitationMetadata
    /// </summary>
    public partial record CitationMetadata
    {
        public CitationSource[]? sources;
    }

    partial record CitationMetadata
    {
        /// <summary>
        /// A source attribution for a piece of content.
        /// </summary>
        public record CitationSource
        {
            public int? startIndex;
            public int? endIndex;
            public string? uri;
            public string? license;
        }
    }
}