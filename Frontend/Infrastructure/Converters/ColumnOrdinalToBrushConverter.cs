using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Frontend.Infrastructure.Converters
{
    /// <summary>
    /// Converts a column ordinal (0/1/2) to a distinct accent color for that column's
    /// header, so backlog / in progress / done are visually distinguishable at a glance -
    /// this is purely cosmetic and makes it obvious where a task currently is.
    /// </summary>
    // A- fields/methods here are instance members, not static ("Classes, class members, and
    // A- methods should not be static, except for loggers") - WPF creates one converter instance
    // A- per XAML resource declaration, so these are still only ever built once per View.
    public class ColumnOrdinalToBrushConverter : IValueConverter
    {
        private readonly Brush _backlog;
        private readonly Brush _inProgress;
        private readonly Brush _done;

        /// <summary>Builds the three frozen, reusable column-accent brushes.</summary>
        public ColumnOrdinalToBrushConverter()
        {
            _backlog = FreezeBrush(0x90, 0x9C, 0xB0);
            _inProgress = FreezeBrush(0xE0, 0x9B, 0x2D);
            _done = FreezeBrush(0x43, 0xA0, 0x47);
        }

        private Brush FreezeBrush(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as int?) switch
            {
                0 => _backlog,
                1 => _inProgress,
                2 => _done,
                _ => _backlog
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
