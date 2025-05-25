using System.Drawing;
using System.Windows.Forms;

namespace PySenti.Copilot.Windows.Screens;

public interface IScreenShot
{
    Screen?[] Screens { get; }

    Screen? Selected { get; set; }

    Bitmap? GetScreenshot();
}