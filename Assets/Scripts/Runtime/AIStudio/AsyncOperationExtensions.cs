using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AIStudio
{
    /// <summary>
    /// Minimum async/await support for Unity's AsyncOperation
    /// which is supported from Unity 2023.2
    /// </summary>
    public static class AsyncOperationExtensions
    {
        public static WebRequestAwaitable GetAwaiter(this AsyncOperation asyncOperation)
        {
            return new WebRequestAwaitable(asyncOperation);
        }

        public readonly struct WebRequestAwaitable : INotifyCompletion
        {
            private readonly AsyncOperation asyncOperation;

            public WebRequestAwaitable(AsyncOperation asyncOperation)
            {
                this.asyncOperation = asyncOperation;
            }

            public bool IsCompleted => asyncOperation.isDone;

            public void OnCompleted(Action continuation)
            {
                asyncOperation.completed += _ => continuation();
            }

            public void GetResult() { }
        }
    }
}
