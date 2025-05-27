using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PySenti.Copilot.App.Converters;

public class RecordingColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isRecording)
        {
            return isRecording ? Brushes.Black : Brushes.Red;
        }

        return Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}