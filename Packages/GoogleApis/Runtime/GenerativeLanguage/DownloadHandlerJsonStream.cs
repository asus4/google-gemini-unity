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
        private char[] textBuffer;

        public DownloadHandlerJsonStream(Action<T> receiveDataDelegate)
        {
            onReceive = receiveDataDelegate;
            textBuffer = new char[2048];
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (dataLength > textBuffer.Length)
            {
                textBuffer = new char[dataLength];
            }

            int readLength = Encoding.UTF8.GetChars(data.AsSpan(0, dataLength), textBuffer);
            Span<char> textSpan = textBuffer.AsSpan(0, readLength);

            // log
            // string text = Encoding.UTF8.GetString(data, 0, dataLength);
            // Log($"ReceiveData: {textSpan.Length} chars, text: {text}");

            if (readLength == 0)
            {
                return true;
            }

            // Beginning of stream
            if (textSpan[0] == '[')
            {
                textSpan = textSpan[1..];
            }
            // Middle of stream
            else if (textSpan[0] == ',')
            {
                textSpan = textSpan[1..];
            }
            // End of stream
            else if (textSpan[^1] == ']')
            {
                textSpan = textSpan[..^1];
            }
            // Log($"Trimmed:\n{text}");

            // Deserialize JSON
            if (!IsNullOrWhitespace(textSpan))
            {
                T obj = JsonExtensions.DeserializeFromJson<T>(new string(textSpan));
                onReceive(obj);
            }
            return true;
        }

        [Conditional("UNITY_EDITOR")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        private static bool IsNullOrWhitespace(ReadOnlySpan<char> span)
        {
            if (span.Length == 0)
            {
                return true;
            }

            foreach (var c in span)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
