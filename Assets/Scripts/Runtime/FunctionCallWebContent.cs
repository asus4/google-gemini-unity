using System.Threading;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace GoogleApis.Example
{
    public sealed class FunctionCallWebContent : MonoBehaviour
    {
        [Preserve]
        [FunctionCall("Get the web page content HTML from the given URL.")]
        public async Task<string> GetWebPage(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            cancellationToken.ThrowIfCancellationRequested();
            return request.downloadHandler.text;
        }
    }
}
