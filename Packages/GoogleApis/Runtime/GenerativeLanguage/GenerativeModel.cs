#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Call API to know all available models for your API Key.
    /// https://generativelanguage.googleapis.com/v1beta/models?key={YOUR_API_KEY}
    /// </summary>
    public static class Models
    {
        // Gemini 1.5
        [Obsolete("Use Gemini 2.0 Pro instead")]
        public const string Gemini_1_5_Pro = "models/gemini-1.5-pro-latest";
        [Obsolete("Use Gemini 2.0 Flash instead")]
        public const string Gemini_1_5_Flash = "models/gemini-1.5-flash-latest";

        // Gemini 2.0
        public const string Gemini_2_0_Flash = "models/gemini-2.0-flash";
        public const string Gemini_2_0_Flash_Lite_Preview = "models/gemini-2.0-flash-lite-preview";
        public const string Gemini_2_0_Flash_Thinking_Exp = "models/gemini-2.0-flash-thinking-exp";
        public const string Gemini_2_0_Pro_Exp = "models/gemini-2.0-pro-exp";

        // Imagen
        public const string Imagen_3_0 = "models/imagen-3.0-generate-002";
    }

    public sealed class GenerativeModel
    {
        public readonly string ModelName;
        private readonly string apiKey;

        internal GenerativeModel(string modelName, string apiKey)
        {
            ModelName = modelName;
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Call generate content API
        /// https://ai.google.dev/api/rest/v1beta/models/generateContent
        /// </summary>
        /// <param name="requestBody">Request data</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A GenerateContentResponse</returns>
        public async UniTask<GenerateContentResponse> GenerateContentAsync(
            GenerateContentRequest requestBody,
            CancellationToken cancellationToken)
        {
            string uri = $"{GenerativeAIClient.BASE_URL}/{ModelName}:generateContent?key={apiKey}";
            return await Api.PostJsonAsync<GenerateContentRequest, GenerateContentResponse>(
                uri, requestBody, cancellationToken);
        }

        /// <summary>
        /// Call generate content API with streaming
        /// https://ai.google.dev/api/rest/v1beta/models/streamGenerateContent
        /// </summary>
        /// <param name="requestBody">A request data</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <param name="onReceive"></param>
        /// <returns></returns>
        public async UniTask StreamGenerateContentAsync(
            GenerateContentRequest requestBody,
            CancellationToken cancellationToken,
            Action<GenerateContentResponse> onReceive)
        {
            string uri = $"{GenerativeAIClient.BASE_URL}/{ModelName}:streamGenerateContent?key={apiKey}";
            string json = requestBody.SerializeToJson();
            Api.Log($"request: {uri},\ndata: {json}");
            using var request = UnityWebRequest.Post(
                uri: uri,
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

        /// <summary>
        /// Call generate image API. Only "imagen-3.0-generate-002" model is supported for now.
        /// https://github.com/google-gemini/cookbook/blob/main/quickstarts/Get_started_imagen_rest.ipynb
        /// </summary>
        /// <param name="requestBody">A request data</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A GenerateImageResponse</returns>
        public async UniTask<GenerateImageResponse> GenerateImageAsync(
            GenerateImageRequest requestBody,
            CancellationToken cancellationToken)
        {
            string uri = $"{GenerativeAIClient.BASE_URL}/{ModelName}:predict?key={apiKey}";
            return await Api.PostJsonAsync<GenerateImageRequest, GenerateImageResponse>(
                uri, requestBody, cancellationToken);
        }
    }
}
