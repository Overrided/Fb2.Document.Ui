using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace RichTextView.WinUI.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                var result = boolValue ? Visibility.Visible : Visibility.Collapsed;
                return result;
            }

            throw new ArgumentException(nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
