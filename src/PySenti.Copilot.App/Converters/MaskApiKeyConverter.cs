using System.Globalization;
using System.Windows.Data;

namespace PySenti.Copilot.App.Converters
{
    public class MaskApiKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string apiKey && !string.IsNullOrEmpty(apiKey))
            {
                // Show first 3 characters and last 3 characters, mask the rest with asterisks
                if (apiKey.Length <= 6)
                {
                    return new string('*', apiKey.Length);
                }

                return $"{apiKey.Substring(0, 3)}...{apiKey.Substring(apiKey.Length - 3)}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported for MaskApiKeyConverter");
        }
    }
}