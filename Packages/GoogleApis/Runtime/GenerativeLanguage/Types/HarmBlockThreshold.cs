using System.Runtime.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/generate-content#v1beta.SafetySetting
    /// </summary>
    public enum HarmBlockThreshold
    {
        [EnumMember(Value = "HARM_BLOCK_THRESHOLD_UNSPECIFIED")]
        Unspecified, // using System.Runtime.Serialization;
        [EnumMember(Value = "BLOCK_LOW_AND_ABOVE")]
        Low, // Content with NEGLIGIBLE will be allowed.
        [EnumMember(Value = "BLOCK_MEDIUM_AND_ABOVE")]
        Medium, // Content with NEGLIGIBLE and LOW will be allowed.
        [EnumMember(Value = "BLOCK_ONLY_HIGH")]
        High, // Content with NEGLIGIBLE, LOW, and MEDIUM will be allowed.
        [EnumMember(Value = "BLOCK_NONE")] None, // All content will be allowed.
    } 
}
