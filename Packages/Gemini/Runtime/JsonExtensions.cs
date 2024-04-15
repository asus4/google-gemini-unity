using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Gemini
{
    /// <summary>
    /// Converting object <-> JSON
    /// 
    /// TODO: Consider using System.Text.Json in .NET 5
    /// for better performance instead of Newtonsoft.Json
    /// </summary>
    internal static class JsonExtensions
    {
        // Remove null values from the JSON 
        private static readonly JsonSerializerSettings settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        private static readonly JsonSerializer serializer = JsonSerializer.CreateDefault(settings);
        private static readonly StringBuilder sb = new(256);

        public static string SerializeToJson<T>(this T value, bool prettyPrint = false)
        {
            sb.Clear();

            Formatting formatting = prettyPrint
                ? Formatting.Indented
                : Formatting.None;
            using StringWriter stringWriter = new(sb, CultureInfo.InvariantCulture);
            using JsonTextWriter jsonTextWriter = new(stringWriter);
            jsonTextWriter.Formatting = formatting;

            serializer.Formatting = formatting;
            serializer.Serialize(jsonTextWriter, value, value.GetType());
            return stringWriter.ToString();
        }

        public static T DeserializeFromJson<T>(this string json)
        {
            T obj = serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)))
                ?? throw new Exception("Failed to deserialize JSON");
            return obj;
        }
    }
}
