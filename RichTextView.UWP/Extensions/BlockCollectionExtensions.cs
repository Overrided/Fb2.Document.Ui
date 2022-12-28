using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace RichTextView.UWP.Extensions
{
    public static class BlockCollectionExtensions
    {
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
