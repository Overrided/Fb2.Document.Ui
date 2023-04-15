using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Fb2.Document.WPF.Playground.Converters;

internal class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;

        var isNumber = value is IConvertible;
        if (!isNumber)
            return Visibility.Collapsed;

        var numberResult = System.Convert.ToDouble(value);

        if (parameter != null &&
            bool.TryParse(parameter.ToString(), out var boolInvertedParam)) // has param
        {
            if (boolInvertedParam) // inverted
                return numberResult > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        return numberResult > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
