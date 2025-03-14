#nullable enable

using System.Text.Json.Serialization;

// Other types
namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Safety setting, affecting the safety-blocking behavior.
    ///
    /// https://ai.google.dev/api/generate-content#v1beta.SafetyRating
    /// </summary>
    public partial record SafetySetting
    {
        public HarmCategory category;
        public HarmBlockThreshold threshold;
    }
}
