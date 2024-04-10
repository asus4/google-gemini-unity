using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GenerativeAI
{
    public sealed class GenerativeModel
    {
        private readonly string uriGenerateContent;

        internal GenerativeModel(string modelName, string apiKey)
        {
            uriGenerateContent = $"{GenerativeAIClient.BASE_URL}/{modelName}:generateContent?key={apiKey}";
        }

        public async Task<string> GenerateContent(GenerateContentRequest requestBody, CancellationToken cancellationToken)
        {
            string json = requestBody.ToJson();
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
            return response;
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
