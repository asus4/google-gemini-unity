using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
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
    internal sealed class DownloadHandlerJsonStream<T> : DownloadHandlerScript
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

            if (readLength == 0)
            {
                return true;
            }

            // Debug Log
            // Log(Encoding.UTF8.GetString(data, 0, dataLength));

            int jsonStart = -1;
            int depth = 0;

            for (int i = 0; i < textSpan.Length; i++)
            {
                switch (textSpan[i])
                {
                    case '{':
                        if (depth == 0)
                        {
                            jsonStart = i;
                        }
                        depth++;
                        break;
                    case '}':
                        depth--;
                        if (depth == 0)
                        {
                            ParseJson(textSpan.Slice(jsonStart, i - jsonStart + 1));
                            jsonStart = -1;
                        }
                        break;
                }
            }
            return true;
        }

        private void ParseJson(ReadOnlySpan<char> span)
        {
            T obj = JsonExtensions.DeserializeFromJson<T>(span);
            onReceive(obj);
        }

        [Conditional("UNITY_EDITOR")]
        [HideInCallstack]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
