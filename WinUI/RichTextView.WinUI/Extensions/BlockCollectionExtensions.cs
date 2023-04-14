using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Documents;

namespace RichTextView.WinUI.Extensions
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
