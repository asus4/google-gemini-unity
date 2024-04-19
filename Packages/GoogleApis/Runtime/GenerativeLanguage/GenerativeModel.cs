#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GoogleApis.GenerativeLanguage
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
        private readonly string uriStreamGenerateContent;
        public readonly string ModelName;

        public bool SupportsSystemInstruction => ModelName switch
        {
            Models.GeminiPro => false,
            Models.GeminiProVision => false,
            Models.Gemini_1_5_Pro => true,
            Models.Gemini_1_5_ProVision => true,
            _ => false,
        };

        internal GenerativeModel(string modelName, string apiKey)
        {
            ModelName = modelName;
            uriGenerateContent = $"{GenerativeAIClient.BASE_URL}/{modelName}:generateContent?key={apiKey}";
            uriStreamGenerateContent = $"{GenerativeAIClient.BASE_URL}/{modelName}:streamGenerateContent?key={apiKey}";
        }

        /// <summary>
        /// Call generate content API
        /// https://ai.google.dev/api/rest/v1beta/models/generateContent
        /// </summary>
        /// <param name="requestBody">Request data</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A generate response</returns>
        public async Task<GenerateContentResponse> GenerateContentAsync(
            GenerateContentRequest requestBody,
            CancellationToken cancellationToken)
        {
            return await Api.PostJsonAsync<GenerateContentRequest, GenerateContentResponse>(
                uriGenerateContent, requestBody, cancellationToken);
        }

        /// <summary>
        /// Call generate content API with streaming
        /// https://ai.google.dev/api/rest/v1beta/models/streamGenerateContent
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="onReceive"></param>
        /// <returns></returns>
        public async Task StreamGenerateContentAsync(
            GenerateContentRequest requestBody,
            CancellationToken cancellationToken,
            Action<GenerateContentResponse> onReceive)
        {
            string json = requestBody.SerializeToJson();
            Api.Log($"request: {uriStreamGenerateContent},\ndata: {json}");

            using var request = UnityWebRequest.Post(
                uri: uriStreamGenerateContent,
                postData: json,
                contentType: "application/json"
            );

            using var downloadHandler = new DownloadHandlerJsonStream<GenerateContentResponse>(onReceive);
            request.downloadHandler = downloadHandler;

            await request.SendWebRequest();
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"code={request.responseCode}, result={request.result}, error={request.error}");
            }
            Api.Log($"Finished streaming");
        }
    }
}
