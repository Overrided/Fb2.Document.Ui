using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace RichTextView.UWP.DTOs
{
    public sealed class RichContentPage : List<TextElement>
    {
        public RichContentPage(IEnumerable<TextElement> collection) : base(collection) { }
    }

    public sealed class ChaptersContent
    {
        public IEnumerable<RichContentPage> RichContentPages { get; } = new List<RichContentPage>();

        public double LeftOffPosition { get; }

        public ChaptersContent(
            IEnumerable<RichContentPage> content,
            double leftOffPosition = 0)
        {
            RichContentPages = content;
            LeftOffPosition = leftOffPosition;
        }

        public bool IsEmpty()
        {
            var empty = RichContentPages == null
                || !RichContentPages.Any()
                || RichContentPages.All(l => l == null || !l.Any());

            return empty;
        }
    }
}
