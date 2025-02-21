using System.Runtime.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// https://ai.google.dev/api/generate-content#v1beta.SafetySetting
    /// </summary>
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
