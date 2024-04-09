namespace GenerativeAI
{
    public sealed class GenerativeModel
    {
        public string Name { get; }
        public string Description { get; }
        public string Language { get; }
        public string Model { get; }

        public GenerativeModel(string name, string description, string language, string model)
        {
            Name = name;
            Description = description;
            Language = language;
            Model = model;
        }
    }
}
