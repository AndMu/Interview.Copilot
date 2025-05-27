using System.Drawing;
using System.Windows.Media.Imaging;

namespace PySenti.Copilot.App.Helpers;

public interface IImageHelper
{
    /// <summary>
    /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
    /// </summary>
    /// <param name="src">A bitmap image</param>
    /// <returns>The image as a BitmapImage for WPF</returns>
    BitmapImage Convert(Bitmap src);

    byte[] Serialise(Bitmap src);
}