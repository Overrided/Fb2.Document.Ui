using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Fb2.Document.WPF.Playground.Converters;

public class MoreThanValueToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int intVal)
            throw new ArgumentException(nameof(value));

        var parameterVal = System.Convert.ToInt32(parameter);

        var result = intVal > parameterVal ? Visibility.Visible : Visibility.Collapsed;
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}