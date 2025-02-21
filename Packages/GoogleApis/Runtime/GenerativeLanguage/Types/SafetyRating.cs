#nullable enable

using System.Text.Json.Serialization;
using System.Runtime.Serialization;

// Other types
namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Safety rating for a piece of content.
    /// 
    /// https://ai.google.dev/api/rest/v1beta/SafetyRating
    /// </summary>
    public partial record SafetyRating
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HarmCategory category;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HarmProbability probability;
        public bool? blocked;
    }

    partial record SafetyRating
    {
        public enum HarmProbability
        {
            HARM_PROBABILITY_UNSPECIFIED,
            NEGLIGIBLE,
            LOW,
            MEDIUM,
            HIGH,
        }
    }
}
