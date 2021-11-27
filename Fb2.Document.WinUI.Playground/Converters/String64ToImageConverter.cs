using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
//using DataConvert = System.Convert; // fucking method name collision lol

namespace Fb2.Document.WinUI.Playground.Converters
{
    public class String64ToImageConverter : IValueConverter
    {
        private const string DefaultImageResoureNameString = "DefaultBookImage";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultBitmap = (BitmapImage)Application.Current.Resources[DefaultImageResoureNameString];

            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                var base64ImageContent = value.ToString();
                return ConvertBase64StringToBitmapImage(base64ImageContent) ?? defaultBitmap;
            }

            return defaultBitmap;
        }

        private BitmapImage ConvertBase64StringToBitmapImage(string base64ImageContent)
        {
            try
            {
                var bitmap = new BitmapImage();

                using (var stream = new InMemoryRandomAccessStream())
                {
                    byte[] bytes = System.Convert.FromBase64String(base64ImageContent);

                    var dataWriter = new DataWriter(stream); // TODO: different writer?
                    dataWriter.WriteBytes(bytes);

                    dataWriter.StoreAsync();
                    stream.FlushAsync();

                    stream.Seek(0);
                    bitmap.SetSource(stream);
                }

                if (bitmap.PixelHeight == 0 || bitmap.PixelWidth == 0)
                    return null;

                if (bitmap.IsAnimatedBitmap) // gifs and stuff
                    bitmap.AutoPlay = true;

                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
