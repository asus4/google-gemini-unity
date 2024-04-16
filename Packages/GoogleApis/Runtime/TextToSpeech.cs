using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GoogleApis.GenerativeLanguage.TTS
{
    /// <summary>
    /// Text to Speech Client using Google Cloud Text-to-Speech API
    /// https://cloud.google.com/text-to-speech/docs/reference/rest
    /// </summary>
    public sealed class TextToSpeech
    {
        private const string BASE_URL = "https://texttospeech.googleapis.com/v1beta1";
        private readonly string uriVoicesList;

        public TextToSpeech(string apiKey)
        {
            uriVoicesList = $"{BASE_URL}/voices?key={apiKey}";
        }

        public TextToSpeech(GoogleApiSettings settings) : this(settings.apiKey)
        {
        }

        public async Task<string> ListVoicesAsync(string languageCode, CancellationToken cancellationToken)
        {
            string url = string.IsNullOrWhiteSpace(languageCode)
                ? uriVoicesList
                : $"{uriVoicesList}&languageCode={languageCode}";
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
            return request.downloadHandler.text;
        }
    }
}
