using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.Extensions
{
    public static class InlineCollectionExtensions
    {
        public static void AddRange(this InlineCollection inlineCollection, IEnumerable<TextElement> inlines)
        {
            foreach (var inlineItem in inlines)
            {
                if (!(inlineItem is Inline))
                    throw new ArgumentException($"{nameof(inlineItem)} is not Inline! Element content : {inlineItem}");

                inlineCollection.Add(inlineItem as Inline);
            }
        }

        public static void AddRange(this BlockCollection blockCollection, IEnumerable<TextElement> blocks)
        {
            foreach (var blockItem in blocks)
            {
                if (!(blockItem is Block))
                    throw new ArgumentException($"{nameof(blockItem)} is not Block! Element content : {blockItem}");

                blockCollection.Add(blockItem as Block);
            }
        }
    }
}
