using System.Runtime.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/rest/v1beta/HarmCategory
    /// </summary>
    public enum HarmCategory
    {
        [EnumMember(Value = "HARM_CATEGORY_UNSPECIFIED")]
        Unspecified, //   Category is unspecified.
        [EnumMember(Value = "HARM_CATEGORY_DEROGATORY")]
        Derogatory, //    Negative or harmful comments targeting identity and/or protected attribute.
        [EnumMember(Value = "HARM_CATEGORY_TOXICITY")]
        Toxicity, // Content that is rude, disrespectful, or profane.
        [EnumMember(Value = "HARM_CATEGORY_VIOLENCE")]
        Violence, //  Describes scenarios depicting violence against an individual or group, or general descriptions of gore.
        [EnumMember(Value = "HARM_CATEGORY_SEXUAL")]
        Sexual, // Contains references to sexual acts or other lewd content.
        [EnumMember(Value = "HARM_CATEGORY_MEDICAL")]
        Medical, // Promotes unchecked medical advice.
        [EnumMember(Value = "HARM_CATEGORY_DANGEROUS")]
        Dangerous, // Dangerous content that promotes, facilitates, or encourages harmful acts.
        [EnumMember(Value = "HARM_CATEGORY_HARASSMENT")]
        Harassment, // Harasment content.
        [EnumMember(Value = "HARM_CATEGORY_HATE_SPEECH")]
        HateSpeech, // Hate speech and content.
        [EnumMember(Value = "HARM_CATEGORY_SEXUALLY_EXPLICIT")]
        SexuallyExplicit, // Sexually explicit content.
        [EnumMember(Value = "HARM_CATEGORY_DANGEROUS_CONTENT")]
        DangerousContent, // Dangerous content.
    }
}
