using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Fb2.Document.UWP.Playground.Converters
{
    public class MoreThanValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int intVal))
                throw new ArgumentException(nameof(value));

            var parameterVal = System.Convert.ToInt32(parameter);

            var result = intVal > parameterVal ? Visibility.Visible : Visibility.Collapsed;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
