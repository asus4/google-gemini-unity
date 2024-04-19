#nullable enable

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GoogleApis
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
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
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

        public static object? JsonCastTo(this object value, Type type)
        {
            if (value == null)
            {
                return null;
            }
            switch (type)
            {
                case Type t when t.IsEnum:
                    return Enum.Parse(t, value.ToString());
                case Type t when t == typeof(string):
                    return value.ToString();
                case Type t when t.IsPrimitive:
                    return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
            }
            // else
            string json = value.SerializeToJson();
            return json.DeserializeFromJson(type);
        }

        private static string SerializeToJson(this object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            sb.Clear();
            using StringWriter stringWriter = new(sb, CultureInfo.InvariantCulture);
            using JsonTextWriter jsonTextWriter = new(stringWriter);
            serializer.Serialize(jsonTextWriter, value, value.GetType());
            return stringWriter.ToString();
        }

        private static object DeserializeFromJson(this string json, Type type)
        {
            object obj = serializer.Deserialize(new JsonTextReader(new StringReader(json)), type)
                ?? throw new Exception("Failed to deserialize JSON");
            return obj;
        }
    }
}
