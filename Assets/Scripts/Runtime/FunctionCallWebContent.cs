using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace GoogleApis.Example
{
    [Preserve]
    public sealed class FunctionCallWebContent : MonoBehaviour
    {
        [Description("Options for GetWebPage method.")]
        public class GetWebPageOptions
        {
            [Description("The URL of the web page.")]
            public string url;
        }

        [Description("Get the web page content HTML from the given URL.")]
        public async Task<string> GetWebPage(GetWebPageOptions options, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Get(options.url);
            await request.SendWebRequest();
            cancellationToken.ThrowIfCancellationRequested();
            return request.downloadHandler.text;
        }
    }
}
