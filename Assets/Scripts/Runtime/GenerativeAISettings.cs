using System;
using System.Linq;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace GenerativeAI
{
    /// <summary>
    /// Settings for GenerativeAI
    /// </summary>
    [Serializable]
    public sealed class GenerativeAISettings : ScriptableObject
    {
        [SerializeField]
        internal string apiKey;


        public static GenerativeAISettings Get()
        {
#if UNITY_EDITOR
            var settings = AssetDatabase.LoadAssetAtPath<GenerativeAISettings>("Assets/Resources/GenerativeAISettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<GenerativeAISettings>();

                // Load from env
                var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                var envFile = File.ReadAllText(envPath);
                settings.apiKey = FromEnvText(envFile);

                AssetDatabase.CreateAsset(settings, "Assets/Resources/GenerativeAISettings.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("Created GenerativeAISettings.asset");
            }
#endif // UNITY_EDITOR
            return Resources.Load<GenerativeAISettings>("GenerativeAISettings");
        }

        private static string FromEnvText(string text)
        {
            var dict = text
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split('='))
                .ToDictionary(parts => parts[0], parts => parts[1]);
            if (!dict.TryGetValue("API_KEY", out string apiKey))
            {
                throw new Exception("API_KEY not found in .env file");
            }
            return apiKey;
        }
    }
}
