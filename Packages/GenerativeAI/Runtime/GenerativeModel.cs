#nullable enable

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GenerativeAI
{

    /// <summary>
    /// https://ai.google.dev/api/rest/v1beta/models/generateContent
    /// </summary>
    public record GenerateContentRequest
    {
        /// <summary>
        /// Required. The content of the current conversation with the model.
        /// 
        /// For single-turn queries, this is a single instance. For multi-turn queries, this is a repeated field that contains conversation history + latest request.
        /// </summary>
        public Content[] contents;
        public Tool[]? tools;

        public GenerateContentRequest(string text)
        {
            contents = new Content[]
            {
                new()
                {
                    parts = new Content.Part[]
                    {
                        new() { text = text, },
                    },
                },
            };
        }
    }

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
