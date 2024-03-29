﻿using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using DataConvert = System.Convert; // method name collision

namespace Fb2.Document.WinUI.Playground.Converters;

public class String64ToImageConverter : IValueConverter
{
    private const string DefaultImageResourceNameString = "DefaultBookImage";

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var defaultBitmap = (BitmapImage)Application.Current.Resources[DefaultImageResourceNameString];

        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
        {
            var base64ImageContent = value.ToString();
            return ConvertBase64StringToBitmapImage(base64ImageContent) ?? defaultBitmap;
        }

        return defaultBitmap;
    }

    private BitmapImage? ConvertBase64StringToBitmapImage(string base64ImageContent)
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
