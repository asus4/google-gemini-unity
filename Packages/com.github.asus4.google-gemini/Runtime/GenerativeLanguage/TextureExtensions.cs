using Cysharp.Threading.Tasks;
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

        public static Blob ToJpgBlob(this Texture2D texture, int quality = 75)
        {
            Assert.IsTrue(texture.isReadable, "Texture2D must be marked as readable");

            var bytes = texture.EncodeToJPG(quality);
            return new Blob(MIME_JPEG, bytes);
        }

        public static async UniTask<Blob> ToJpgBlobAsync(this Texture texture, int quality = 75)
        {
            if (texture is Texture2D texture2D && texture2D.isReadable)
            {
                return texture2D.ToJpgBlob(quality);
            }

            int width = texture.width;
            int height = texture.height;

            // This format throws an exception on metal but it actually works...
            GraphicsFormat format = FindSupportedFormat();
            NativeArray<byte> imageBytes = new(width * height * 4, Allocator.Persistent);

            Blob blob = null;
            try
            {
                var request = await AsyncGPUReadback.RequestIntoNativeArray(ref imageBytes, texture, 0, format, (request) =>
                {
                    if (request.hasError)
                    {
                        throw new System.Exception($"AsyncGPUReadback.RequestIntoNativeArray failed: {request}");
                    }
                });
                var jpgBytes = ImageConversion.EncodeArrayToJPG(imageBytes.ToArray(), format, (uint)width, (uint)height, 0, quality);
                blob = new Blob(MIME_JPEG, jpgBytes);
            }
            finally
            {
                imageBytes.Dispose();
            }
            return blob;
        }

        public static Texture2D ToTexture(this Blob blob)
        {
            if (!blob.MimeType.StartsWith("image/"))
            {
                throw new System.ArgumentException($"Unsupported mime type: {blob.MimeType}");
            }
            var texture = new Texture2D(2, 2);
            texture.LoadImage(blob.Data.ToArray());
            return texture;
        }


        static readonly GraphicsFormat[] FORMATS = {
            GraphicsFormat.R8G8B8A8_SRGB,
            GraphicsFormat.R8G8B8A8_UNorm,
            GraphicsFormat.R8G8B8A8_SNorm,
        };

        static GraphicsFormat FindSupportedFormat()
        {
            foreach (var format in FORMATS)
            {
                if (SystemInfo.IsFormatSupported(format, GraphicsFormatUsage.GetPixels))
                {
                    return format;
                }
            }
            throw new System.InvalidOperationException("No supported format found");
        }
    }
}
