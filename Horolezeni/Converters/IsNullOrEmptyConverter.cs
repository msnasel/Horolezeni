using System.Globalization;

namespace Horolezeni.Converters
{
    /// <summary>
    /// Converts a string to a boolean indicating whether the string is null or empty.
    /// </summary>
    public class IsNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || string.IsNullOrEmpty(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}