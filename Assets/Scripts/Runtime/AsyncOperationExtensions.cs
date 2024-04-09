using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GenerativeAI
{
    /// <summary>
    /// Minimum async/await support for Unity's AsyncOperation
    /// which is supported from Unity 2023.2
    /// </summary>
    public static class AsyncOperationExtensions
    {
        public static Awaitable GetAwaiter(this AsyncOperation asyncOperation)
        {
            return new Awaitable(asyncOperation);
        }

        public readonly struct Awaitable : INotifyCompletion
        {
            private readonly AsyncOperation asyncOperation;

            public Awaitable(AsyncOperation asyncOperation)
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