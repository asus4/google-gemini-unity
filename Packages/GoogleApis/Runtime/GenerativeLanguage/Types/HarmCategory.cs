using System.Runtime.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/generate-content#v1beta.HarmCategory
    /// </summary>
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
}
