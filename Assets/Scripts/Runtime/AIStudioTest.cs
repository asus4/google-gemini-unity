using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AIStudioExperiments
{
    public sealed class AIStudioTest : MonoBehaviour
    {
        private AIStudio.Client client;

        private async void Start()
        {
            // TODO: support build
            client = AIStudio.Client.FromEnvFile(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
            Debug.Log($"Client: {client}");

            var models = await client.ListModels(destroyCancellationToken);
            Debug.Log($"Available models: {models}");
        }
    }
}
