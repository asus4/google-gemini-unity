using System;
using System.Diagnostics;
using System.Text;
using UnityEngine.Networking;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Pass streaming JSON data to delegate
    /// 
    /// // format is like this:
    /// [ // start of stream
    ///    { stream object },
    ///    { stream object },
    ///    { stream object },
    /// ] // end of stream
    /// </summary>
    public sealed class DownloadHandlerJsonStream<T> : DownloadHandlerScript
    {
        private readonly Action<T> onReceive;

        public DownloadHandlerJsonStream(Action<T> receiveDataDelegate)
        {
            onReceive = receiveDataDelegate;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            string text = Encoding.UTF8.GetString(data, 0, dataLength);
            // Log($"ReceiveData:\n{text}");

            // Beginning of stream
            if (text.StartsWith('['))
            {
                text = text.TrimStart('[');
            }
            // Middle of stream
            else if (text.StartsWith(','))
            {
                text = text.TrimStart(',');
            }
            // End of stream
            else if (text.EndsWith(']'))
            {
                text = text.TrimEnd(']');
            }
            // Log($"Trimmed:\n{text}");

            // Deserialize JSON
            if (!string.IsNullOrWhiteSpace(text))
            {
                T obj = JsonExtensions.DeserializeFromJson<T>(text);
                onReceive(obj);
            }
            return true;
        }

        [Conditional("UNITY_EDITOR")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
