using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Converting Texture to Base64 Blob
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

            int width = texture.width;
            int height = texture.height;
            const GraphicsFormat format = GraphicsFormat.R8G8B8_SRGB;
            NativeArray<byte> imageBytes = new(width * height * 3, Allocator.Persistent);

            Content.Blob blob = null;
            try
            {
                bool isDone = false;
                AsyncGPUReadbackRequest request = AsyncGPUReadback.RequestIntoNativeArray(ref imageBytes, texture, 0, format, (request) =>
                {
                    isDone = true;
                    if (request.hasError)
                    {
                        throw new System.Exception($"AsyncGPUReadback.RequestIntoNativeArray failed: {request}");
                    }
                });

                while (!isDone)
                {
                    await Task.Yield();
                }
                using NativeArray<byte> jpgBytes = ImageConversion.EncodeNativeArrayToJPG(imageBytes, format, (uint)width, (uint)height, 0, quality);
                blob = new Content.Blob(MIME_JPEG, jpgBytes.AsReadOnlySpan());
            }
            finally
            {
                imageBytes.Dispose();
            }
            return blob;
        }
    }
}
