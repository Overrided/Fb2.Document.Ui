using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;

namespace RichTextView.UWP.Services
{
    public static class ContainerBuilder
    {
        public static RichTextBlock BuildRichTextBlock(double baseFontSize, Size viewPortSize)
        {
            return new RichTextBlock
            {
                TextWrapping = TextWrapping.WrapWholeWords,
                TextTrimming = TextTrimming.None,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                FontSize = baseFontSize,
                MinHeight = GetRichTextBlockMinHeight(viewPortSize) // TODO : define,
            };
        }

        public static double GetRichTextBlockMinHeight(Size viewPortSize) => viewPortSize.Height / 2.3;

        public static RichTextBlockOverflow BuildOverflow(Size viewPortSize)
            => new RichTextBlockOverflow
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MinHeight = GetRichTextBlockMinHeight(viewPortSize)
            };
    }
}
