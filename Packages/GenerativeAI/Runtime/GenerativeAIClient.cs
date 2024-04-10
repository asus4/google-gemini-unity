using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GenerativeAI
{
    /// <summary>
    /// (Non official) REST API client for Generative Language API 
    /// 
    /// See API reference here:
    /// https://ai.google.dev/api/rest
    /// </summary>
    public sealed class GenerativeAIClient
    {
        internal const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta";
        internal readonly string apiKey;

        public GenerativeAIClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public GenerativeAIClient(GenerativeAISettings settings) : this(settings.apiKey)
        {
        }

        /// <summary>
        /// Return a list of available models
        /// </summary>
        public async ValueTask<string> ListModels(CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get($"{BASE_URL}/models?key={apiKey}");
            await request.SendWebRequest();
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }
            return request.downloadHandler.text;
        }

        public GenerativeModel GetModel(string modelName)
        {
            return new GenerativeModel(modelName, apiKey);
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        private void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }

    public sealed class GenerativeModel
    {
        public readonly string modelName;
        private readonly string apiKey;

        public GenerativeModel(string modelName, string apiKey)
        {
            this.modelName = modelName;
            this.apiKey = apiKey;
        }

        public async ValueTask<string> Chat(string text, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.PostWwwForm($"{GenerativeAIClient.BASE_URL}/models/{modelName}:chat?key={apiKey}", "");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(text));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }
            return request.downloadHandler.text;
        }
    }
}
