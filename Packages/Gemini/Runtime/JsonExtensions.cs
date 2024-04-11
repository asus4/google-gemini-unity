using System;
using Newtonsoft.Json;

namespace Gemini
{
    internal static class JsonExtensions
    {
        // Remove null values from the JSON 
        private static readonly JsonSerializerSettings Settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static string SerializeToJson<T>(this T requestBody, bool prettyPrint = false)
        {
            Formatting formatting = prettyPrint
                ? Formatting.Indented
                : Formatting.None;
            return JsonConvert.SerializeObject(requestBody, formatting, Settings);
        }

        public static T DeserializeFromJson<T>(this string json)
        {
            T obj = JsonConvert.DeserializeObject<T>(json, Settings)
                ?? throw new Exception("Failed to deserialize JSON");
            return obj;
        }
    }
}
