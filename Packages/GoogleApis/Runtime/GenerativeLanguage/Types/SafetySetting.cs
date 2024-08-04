#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonConverter(typeof(StringEnumConverter))]
        public HarmCategory category;
        [JsonConverter(typeof(StringEnumConverter))]
        public HarmBlockThreshold threshold;
    }
}
