#nullable enable

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GenerativeAI
{
    /// <summary>
    /// Call API to know all available models for your API Key.
    /// https://generativelanguage.googleapis.com/v1beta/models?key={YOUR_API_KEY}
    /// </summary>
    public static class Models
    {
        public const string GeminiPro = "models/gemini-pro";
        public const string GeminiProVision = "models/gemini-pro-vision";

        public const string Gemini_1_5_Pro = "models/gemini-1.5-pro-latest";
        public const string Gemini_1_5_ProVision = "models/gemini-1.5-pro-vision-latest";
    }

    public sealed class GenerativeModel
    {
        private readonly string uriGenerateContent;

        internal GenerativeModel(string modelName, string apiKey)
        {
            uriGenerateContent = $"{GenerativeAIClient.BASE_URL}/{modelName}:generateContent?key={apiKey}";
        }

        public async Task<GenerateContentResponse> GenerateContentAsync(GenerateContentRequest requestBody, CancellationToken cancellationToken)
        {
            string json = requestBody.SerializeToJson();
            Log($"request: {uriGenerateContent},\ndata: {json}");

            using var request = UnityWebRequest.Post(
                uri: uriGenerateContent,
                postData: json,
                contentType: "application/json"
            );
            await request.SendWebRequest();
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }

            string response = request.downloadHandler.text;
            Log($"response: {response}");
            return response.DeserializeFromJson<GenerateContentResponse>();
        }

        [Conditional("UNITY_EDITOR")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
