using System.Drawing;
using System.Windows.Forms;

namespace PySenti.Copilot.Windows.Screens
{
    public class ScreenShot : IScreenShot
    {
        public Screen?[] Screens => Screen.AllScreens;

        public Screen? Selected { get; set; }

        public Bitmap? GetScreenshot()
        {
            if (Selected == null)
            {
                return null;
            }

            Bitmap bitmap = new Bitmap(Selected.Bounds.Width, Selected.Bounds.Height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(Selected.Bounds.X, Selected.Bounds.Y, 0, 0, Selected.Bounds.Size);
            return bitmap;
        }
    }
}
