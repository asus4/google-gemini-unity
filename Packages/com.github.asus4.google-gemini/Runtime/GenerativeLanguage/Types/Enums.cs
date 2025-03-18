using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/generate-content#Modality
    /// </summary> 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Modality
    {
        MODALITY_UNSPECIFIED,
        TEXT,
        IMAGE,
        VIDEO,
        AUDIO,
        DOCUMENT,
    }

    /// <summary>
    /// https://ai.google.dev/api/generate-content#HarmProbability
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HarmProbability
    {
        HARM_PROBABILITY_UNSPECIFIED,
        NEGLIGIBLE,
        LOW,
        MEDIUM,
        HIGH,
    }

    /// <summary>
    /// https://ai.google.dev/api/generate-content#v1beta.HarmCategory
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HarmCategory
    {
        HARM_CATEGORY_UNSPECIFIED, //   Category is unspecified.
        HARM_CATEGORY_DEROGATORY, //    Negative or harmful comments targeting identity and/or protected attribute.
        HARM_CATEGORY_TOXICITY, // Content that is rude, disrespectful, or profane.
        HARM_CATEGORY_VIOLENCE, //  Describes scenarios depicting violence against an individual or group, or general descriptions of gore.
        HARM_CATEGORY_SEXUAL, // Contains references to sexual acts or other lewd content.
        HARM_CATEGORY_MEDICAL, // Promotes unchecked medical advice.
        HARM_CATEGORY_DANGEROUS, // Dangerous content that promotes, facilitates, or encourages harmful acts.
        HARM_CATEGORY_HARASSMENT, // Gemini - Harassment content.
        HARM_CATEGORY_HATE_SPEECH, // Gemini - Hate speech and content.
        HARM_CATEGORY_SEXUALLY_EXPLICIT, // Gemini - Sexually explicit content.
        HARM_CATEGORY_DANGEROUS_CONTENT, // Gemini - Dangerous content.
        HARM_CATEGORY_CIVIC_INTEGRITY, // Gemini - Content that may be used to harm civic integrity.
    }

    /// <summary>
    /// hhttps://ai.google.dev/api/generate-content#HarmBlockThreshold
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HarmBlockThreshold
    {
        HARM_BLOCK_THRESHOLD_UNSPECIFIED, // Threshold is unspecified.
        BLOCK_LOW_AND_ABOVE, // Content with NEGLIGIBLE will be allowed.
        BLOCK_MEDIUM_AND_ABOVE, // Content with NEGLIGIBLE and LOW will be allowed.
        BLOCK_ONLY_HIGH, // Content with NEGLIGIBLE, LOW, and MEDIUM will be allowed.
        BLOCK_NONE, // All content will be allowed.
        OFF, // Turn off the safety filter.
    }
}
