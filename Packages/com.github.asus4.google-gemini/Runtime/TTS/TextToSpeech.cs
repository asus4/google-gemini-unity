using System.Threading;
using System.Threading.Tasks;

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
            return await Api.GetJsonAsync<VoicesResponse>(url, cancellationToken);
        }

        /// <summary>
        /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/text/synthesize
        /// </summary>
        /// <param name="message"></param>
        public async Task<TextSynthesizeResponse> SynthesizeAsync(
            TextSynthesizeRequest requestBody, CancellationToken cancellationToken)
        {
            return await Api.PostJsonAsync<TextSynthesizeRequest, TextSynthesizeResponse>(
                uriTextSynthesize, requestBody, cancellationToken);
        }
    }
}
