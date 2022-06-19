using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace RichTextView.Services
{
    public static class ContainerBuilder
    {
        public static RichTextBlock BuildRichTextBlock(double baseFontSize, Size viewPortSize)
        {
            // todo: add validation

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

        private static double GetRichTextBlockMinHeight(Size viewPortSize) => viewPortSize.Height / 2.5;

        public static RichTextBlockOverflow BuildOverflow(Size viewPortSize)
            => new RichTextBlockOverflow
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                MinHeight = GetRichTextBlockMinHeight(viewPortSize)
            };
    }
}
