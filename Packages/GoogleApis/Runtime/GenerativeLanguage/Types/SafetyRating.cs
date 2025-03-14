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
        public HarmCategory category;
        public HarmProbability probability;
        public bool? blocked;
    }
}
