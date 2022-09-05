using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Documents;

namespace RichTextView.DTOs
{
    public sealed class RichContentPage : List<TextElement>
    {
        public RichContentPage(IEnumerable<TextElement> collection) : base(collection) { }
    }

    public sealed class RichContent
    {
        public IEnumerable<RichContentPage> RichContentPages { get; } = new List<RichContentPage>();

        public double LeftOffPosition { get; }

        public HashSet<string> NotInlineImageTags { get; } = new();

        public RichContent(IEnumerable<RichContentPage> content) : this(content, new()) { }

        public RichContent(
            IEnumerable<RichContentPage> content,
            HashSet<string> notInlineImageTags,
            double leftOffPosition = 0)
        {
            RichContentPages = content;
            NotInlineImageTags = notInlineImageTags;
            LeftOffPosition = leftOffPosition;
        }

        public bool IsEmpty()
        {
            var empty = (RichContentPages == null
                || !RichContentPages.Any()
                || RichContentPages.All(l => l == null || !l.Any())) &&
                (NotInlineImageTags == null || NotInlineImageTags.Count == 0);

            return empty;
        }
    }
}
