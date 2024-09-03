namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// A description of a model.
    /// https://ai.google.dev/api/models#Model
    /// </summary>
    public record Model
    {
        public string name;
        public string baseModelId;
        public string version;
        public string displayName;
        public string description;
        public int inputTokenLimit;
        public int outputTokenLimit;
        public string[] supportedGenerationMethods;
        public float temperature;
        public float maxTemperature;
        public float topP;
        public int topK;

        public override string ToString() => this.SerializeToJson(true);
    }

    public record ModelList
    {
        public Model[] models;

        public override string ToString() => this.SerializeToJson(true);
    }
}
