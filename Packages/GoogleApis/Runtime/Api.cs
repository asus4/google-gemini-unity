using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleApis
{
    /// <summary>
    /// Internal utility class for calling APIs.
    /// </summary>
    internal static class Api
    {
        internal static async UniTask<TResponse> GetJsonAsync<TResponse>(
            string uri, CancellationToken cancellationToken)
        {
            Log($"request: {uri}");

            using var request = UnityWebRequest.Get(uri);
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
            return response.DeserializeFromJson<TResponse>();
        }

        internal static async UniTask<TResponse> PostJsonAsync<TRequest, TResponse>(
            string uri, TRequest requestBody, CancellationToken cancellationToken)
        {
            string json = requestBody.SerializeToJson(UnityEngine.Debug.isDebugBuild);
            Log($"request: {uri},\ndata: {json}");

            using var request = UnityWebRequest.Post(
                uri: uri,
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
            return response.DeserializeFromJson<TResponse>();
        }

        [Conditional("UNITY_EDITOR")]
        [HideInCallstack]
        internal static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
