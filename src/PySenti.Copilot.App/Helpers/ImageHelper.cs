using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.IO;

namespace PySenti.Copilot.App.Helpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly RecyclableMemoryStreamManager memoryStreamManager;

        public ImageHelper(RecyclableMemoryStreamManager memoryStreamManager)
        {
            ArgumentNullException.ThrowIfNull(memoryStreamManager);
            this.memoryStreamManager = memoryStreamManager;
        }

        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public BitmapImage Convert(Bitmap src)
        {
            using var stream = memoryStreamManager.GetStream();
            src.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            stream.Seek(0, SeekOrigin.Begin);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze(); // Makes it usable across threads
            return image;
        }

        public byte[] Serialise(Bitmap src)
        {
            using var stream = memoryStreamManager.GetStream();
            src.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var result = stream.ToArray();
            return result;
        }
    }
}
