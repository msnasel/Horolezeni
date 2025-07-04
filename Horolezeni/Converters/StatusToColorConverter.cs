using System.Globalization;
using Microsoft.Maui.Controls;

// Jmenný prostor opraven z Horolezeni.ViewModels na Horolezeni.Converters
namespace Horolezeni.Converters
{
    /// <summary>
    /// Converts a status string to a corresponding color.
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Naplánováno" => Colors.Blue,
                    "Probíhá" => Colors.DarkGreen,
                    "Dokončeno" => Colors.Gray,
                    "Zrušeno" => Colors.Red,
                    "Neznámý stav" => Colors.Orange,
                    "Není k dispozici" => Colors.LightGray,
                    _ => Colors.Black, // Default color for any other status
                };
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This is not needed for this application.
            throw new NotImplementedException();
        }
    }
}
