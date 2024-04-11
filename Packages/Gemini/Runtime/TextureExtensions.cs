using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Gemini
{
    /// <summary>
    /// Utilities to handle Texture with Gemini API
    /// </summary>
    public static class TextureExtensions
    {
        private const string MIME_JPEG = "image/jpeg";

        public static Content.Blob ToJpgBlob(this Texture2D texture, int quality = 75)
        {
            Assert.IsTrue(texture.isReadable, "Texture2D must be marked as readable");

            var bytes = texture.EncodeToJPG(quality);
            return new Content.Blob(MIME_JPEG, bytes);
        }

        public static async Task<Content.Blob> ToJpgBlobAsync(this Texture texture, int quality = 75)
        {
            if (texture is Texture2D texture2D && texture2D.isReadable)
            {
                return texture2D.ToJpgBlob(quality);
            }

            const GraphicsFormat format = GraphicsFormat.R8G8B8_SRGB;
            NativeArray<byte> imageBytes = new(texture.width * texture.height * 3, Allocator.Persistent);

            bool isDone = false;
            AsyncGPUReadbackRequest request = AsyncGPUReadback.RequestIntoNativeArray(ref imageBytes, texture, 0, format, (request) =>
            {
                Debug.Log("AsyncGPUReadback.RequestIntoNativeArray");
                isDone = true;
            });
            // Wait for isDone == true
            await Task.Run(() =>
            {
                while (!isDone) { }
            });

            using NativeArray<byte> jpgBytes = ImageConversion.EncodeNativeArrayToJPG(imageBytes, format, (uint)texture.width, (uint)texture.height, 0, quality);
            imageBytes.Dispose();

            return new Content.Blob(MIME_JPEG, jpgBytes.AsReadOnlySpan());
        }
    }
}
