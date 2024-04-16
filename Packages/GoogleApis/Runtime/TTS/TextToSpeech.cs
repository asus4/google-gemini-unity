using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GoogleApis.TTS
{
    /// <summary>
    /// Text to Speech Client using Google Cloud Text-to-Speech API
    /// https://cloud.google.com/text-to-speech/docs/reference/rest
    /// </summary>
    public sealed class TextToSpeech
    {
        private const string BASE_URL = "https://texttospeech.googleapis.com/v1beta1";
        private readonly string uriVoicesList;
        private readonly string uriTextSynthesize;

        public TextToSpeech(string apiKey)
        {
            uriVoicesList = $"{BASE_URL}/voices?key={apiKey}";
            uriTextSynthesize = $"{BASE_URL}/text:synthesize?key={apiKey}";
        }

        public TextToSpeech(GoogleApiSettings settings) : this(settings.apiKey)
        {
        }

        /// <summary>
        /// Returns a list of Voice supported for synthesis.
        /// 
        /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/voices/list
        /// </summary>
        public async Task<VoicesResponse> ListVoicesAsync(string languageCode, CancellationToken cancellationToken)
        {
            string url = string.IsNullOrWhiteSpace(languageCode)
                ? uriVoicesList
                : $"{uriVoicesList}&languageCode={languageCode}";
            Log($"request: {url}");

            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }

            string json = request.downloadHandler.text;
            Log($"response: {json}");
            return json.DeserializeFromJson<VoicesResponse>();
        }

        /// <summary>
        /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/text/synthesize
        /// </summary>
        /// <param name="message"></param>
        public async Task<TextSynthesizeResponse> SynthesizeAsync(
            TextSynthesizeRequest requestBody, CancellationToken cancellationToken)
        {
            string json = requestBody.SerializeToJson(UnityEngine.Debug.isDebugBuild);
            Log($"request: {uriTextSynthesize},\ndata: {json}");
            using var request = UnityWebRequest.Post(
                uri: uriTextSynthesize,
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
            return response.DeserializeFromJson<TextSynthesizeResponse>();
        }

        [Conditional("UNITY_EDITOR")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
