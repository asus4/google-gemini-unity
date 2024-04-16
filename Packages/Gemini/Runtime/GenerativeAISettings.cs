using System;
using System.Linq;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Gemini
{
    /// <summary>
    /// Settings for GenerativeAI
    /// </summary>
    [Serializable]
    public sealed class GenerativeAISettings : ScriptableObject, IDisposable
    {
        [SerializeField]
        internal string apiKey;

        public void Dispose()
        {
            Resources.UnloadAsset(this);
        }

        public static GenerativeAISettings Get(string key = "API_KEY")
        {
            EnsureSettingExist(key);
            return Resources.Load<GenerativeAISettings>("GenerativeAISettings");
        }

        public static void EnsureSettingExist(string key = "API_KEY")
        {
#if UNITY_EDITOR
            var settings = AssetDatabase.LoadAssetAtPath<GenerativeAISettings>("Assets/Resources/GenerativeAISettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<GenerativeAISettings>();

                // Load from env
                var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                var envFile = File.ReadAllText(envPath);
                settings.apiKey = FromEnvText(envFile, key);

                AssetDatabase.CreateAsset(settings, "Assets/Resources/GenerativeAISettings.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("Created GenerativeAISettings.asset");
            }
#endif // UNITY_EDITOR
        }

        private static string FromEnvText(string text, string key)
        {
            var dict = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('='))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1]);
            if (!dict.TryGetValue(key, out string apiKey))
            {
                throw new Exception($"{key}=YOUR_API_KEY not found in .env file");
            }
            return apiKey;
        }
    }
}
