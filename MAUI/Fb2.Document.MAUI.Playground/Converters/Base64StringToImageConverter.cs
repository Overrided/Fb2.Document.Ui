using System.Globalization;

namespace Fb2.Document.MAUI.Playground.Converters;

public class Base64StringToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        var base64ImageContent = value.ToString();

        byte[] bytes = System.Convert.FromBase64String(base64ImageContent);
        var base64Str = System.Convert.ToBase64String(bytes);

        //using (var ms = new MemoryStream(bytes))
        //{
        var imageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
        return imageSource;
        //}
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
