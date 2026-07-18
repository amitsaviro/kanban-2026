using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Frontend.Infrastructure.Converters
{
    /// <summary>
    /// Converts a bool to <see cref="Visibility.Visible"/>/<see cref="Visibility.Collapsed"/>.
    /// Used to show/hide controls that only make sense for the board owner (e.g. transfer ownership).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
