using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// A description of a model.
    /// https://ai.google.dev/api/models#Model
    /// </summary>
    public record Model
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("baseModelId")]
        public string BaseModelId { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("inputTokenLimit")]
        public int InputTokenLimit { get; set; }
        [JsonPropertyName("outputTokenLimit")]
        public int OutputTokenLimit { get; set; }
        [JsonPropertyName("supportedGenerationMethods")]
        public string[] SupportedGenerationMethods { get; set; }
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }
        [JsonPropertyName("maxTemperature")]
        public float MaxTemperature { get; set; }
        [JsonPropertyName("topP")]
        public float TopP { get; set; }
        [JsonPropertyName("topK")]
        public int TopK { get; set; }

        public override string ToString() => this.SerializeToJson(true);
    }

    public record ModelList
    {
        [JsonPropertyName("models")]
        public Model[] Models { get; set; }

        public override string ToString() => this.SerializeToJson(true);
    }
}
