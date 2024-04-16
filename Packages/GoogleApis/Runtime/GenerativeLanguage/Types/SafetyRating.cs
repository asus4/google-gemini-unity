#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonConverter(typeof(StringEnumConverter))]
        public HarmCategory category;
        [JsonConverter(typeof(StringEnumConverter))]
        public HarmProbability probability;
        public bool? blocked;
    }

    partial record SafetyRating
    {
        public enum HarmProbability
        {
            [EnumMember(Value = "HARM_PROBABILITY_UNSPECIFIED")]
            Unspecified,
            [EnumMember(Value = "NEGLIGIBLE")]
            Negligible,
            [EnumMember(Value = "LOW")]
            Low,
            [EnumMember(Value = "MEDIUM")]
            Medium,
            [EnumMember(Value = "HIGH")]
            High,
        }
    }
}
