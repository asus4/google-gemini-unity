using System;
using Newtonsoft.Json;

namespace GenerativeAI
{
    internal static class JsonExtensions
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static string SerializeToJson<T>(this T requestBody)
        {
            return JsonConvert.SerializeObject(requestBody, Settings);
        }

        public static T DeserializeFromJson<T>(this string json)
        {
            T obj = JsonConvert.DeserializeObject<T>(json, Settings)
                ?? throw new Exception("Failed to deserialize JSON");
            return obj;
        }
    }
}
