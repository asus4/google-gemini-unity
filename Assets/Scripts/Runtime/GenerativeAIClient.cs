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
        private const string BASE_URL = "https://generativelanguage.googleapis.com/v1beta";
        private readonly string apiKey;

        public GenerativeAIClient(string apiKey)
        {
            this.apiKey = apiKey;
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

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        private void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
