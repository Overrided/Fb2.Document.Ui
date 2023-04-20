using System;
using System.Globalization;
using System.Windows.Data;

namespace Fb2.Document.WPF.Playground.Converters;

public class FileSizeInBytesToHumanReadableStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not long longValue)
            throw new ArgumentException();

        var byteCount = longValue;

        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];

        long bytes = Math.Abs(byteCount);
        int place = System.Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 2);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
