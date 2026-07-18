using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Frontend.Infrastructure.Converters
{
    /// <summary>
    /// Converts a string to <see cref="Visibility.Visible"/> when non-empty, or
    /// <see cref="Visibility.Collapsed"/> when null/empty. Used to show error-message
    /// TextBlocks only when there is actually an error to display.
    /// </summary>
    // A- lets XAML hide the error TextBlock declaratively (Visibility="{Binding ErrorMessage,
    // A- Converter={StaticResource StringNotEmptyToVisibility}}") instead of toggling it from code-behind.
    public class StringNotEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
