#nullable enable

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public static string SerializeToJson<T>(this T value, bool prettyPrint = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return JsonSerializer.Serialize(value, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IncludeFields = true,
                WriteIndented = prettyPrint,
            });
        }

        public static T DeserializeFromJson<T>(this ReadOnlySpan<char> json)
        {
            T obj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                IncludeFields = true,
            }) ?? throw new Exception("Failed to deserialize JSON");
            return obj;
        }

        public static T DeserializeFromJson<T>(this string json)
        {
            return DeserializeFromJson<T>(json.AsSpan());
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
            return JsonSerializer.Deserialize(json, type)
                ?? throw new Exception("Failed to cast JSON to target type");
        }
    }
}
