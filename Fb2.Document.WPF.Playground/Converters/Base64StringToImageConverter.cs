using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Fb2.Document.WPF.Playground.Converters;

public class Base64StringToImageConverter : IValueConverter
{
    private const string DefaultImageResourceNameString = "DefaultBookImage";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var defaultBitmap = (BitmapImage)App.Current.Resources[DefaultImageResourceNameString];

        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
        {
            var base64ImageContent = value.ToString();
            return ConvertBase64StringToBitmapImage(base64ImageContent) ?? defaultBitmap;
        }

        return defaultBitmap;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private BitmapImage? ConvertBase64StringToBitmapImage(string? base64ImageContent)
    {
        if (string.IsNullOrEmpty(base64ImageContent))
            return null;

        try
        {
            var bitmap = new BitmapImage();

            byte[] imageBytes = System.Convert.FromBase64String(base64ImageContent);
            using var stream = new MemoryStream(imageBytes);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            if (bitmap.PixelHeight == 0 || bitmap.PixelWidth == 0)
                return null;

            return bitmap;
        }
        catch
        {
            return null;
        }
    }
}
