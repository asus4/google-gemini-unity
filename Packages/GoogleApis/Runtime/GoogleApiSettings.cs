using System;
using System.Linq;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace GoogleApis
{
    /// <summary>
    /// Settings for GenerativeAI
    /// </summary>
    [Serializable]
    public sealed class GoogleApiSettings : ScriptableObject, IDisposable
    {
        [SerializeField]
        internal string apiKey;

        public void Dispose()
        {
            Resources.UnloadAsset(this);
        }

        public static GoogleApiSettings Get(string key = "API_KEY")
        {
            EnsureSettingExist(key);
            return Resources.Load<GoogleApiSettings>("GoogleApiSettings");
        }

        public static void EnsureSettingExist(string key = "API_KEY")
        {
#if UNITY_EDITOR
            const string PATH = "Assets/Resources/GoogleApiSettings.asset";
            var settings = AssetDatabase.LoadAssetAtPath<GoogleApiSettings>(PATH);
            if (settings == null)
            {
                settings = CreateInstance<GoogleApiSettings>();

                // Load from env
                var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
                var envFile = File.ReadAllText(envPath);
                settings.apiKey = FromEnvText(envFile, key);

                AssetDatabase.CreateAsset(settings, PATH);
                AssetDatabase.SaveAssets();
                Debug.Log($"Created {PATH}");
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
