using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace GoogleApis.GenerativeLanguage
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
        private readonly string apiKey;

        public GenerativeAIClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public GenerativeAIClient(GoogleApiSettings settings) : this(settings.apiKey)
        {
        }

        /// <summary>
        /// Return a list of available models
        /// https://ai.google.dev/api/models#method:-models.list 
        /// </summary>
        public async UniTask<ModelList> ListModelsAsync(CancellationToken cancellationToken)
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

            string response = request.downloadHandler.text;
            return response.DeserializeFromJson<ModelList>();
        }

        public GenerativeModel GetModel(string modelName)
        {
            return new GenerativeModel(modelName, apiKey);
        }
    }
}
