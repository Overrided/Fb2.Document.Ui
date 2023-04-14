using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using DataConvert = System.Convert; // fucking method name collision lol

namespace Fb2.Document.UWP.Playground.Converters
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

                using (var stream = new MemoryStream())
                {
                    byte[] imageBytes = DataConvert.FromBase64String(base64ImageContent);
                    stream.Write(imageBytes, 0, imageBytes.Length);

                    var randomAccessStream = stream.AsRandomAccessStream();
                    randomAccessStream.Seek(0);
                    bitmap.SetSource(randomAccessStream);
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
