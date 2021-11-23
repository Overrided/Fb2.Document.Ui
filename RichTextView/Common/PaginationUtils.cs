using System.Collections.Generic;
using Microsoft.UI.Xaml.Documents;

namespace RichTextView.Common
{
    internal static class PaginationUtils
    {
        internal static List<TextElement> Paragraphize(IEnumerable<TextElement> elements)
        {
            Paragraph actualParagraph = null;

            var result = new List<TextElement>();

            foreach (var element in elements)
            {
                if (element is Paragraph paragraphElement)
                {
                    if (actualParagraph != null)
                    {
                        result.Add(actualParagraph);
                        actualParagraph = null;
                    }
                    result.Add(paragraphElement);
                }
                else if (element is Inline inlineElement)
                {
                    if (actualParagraph == null)
                        actualParagraph = new Paragraph();

                    actualParagraph.Inlines.Add(inlineElement);
                }
            }

            if (actualParagraph != null)
                result.Add(actualParagraph);

            return result;
        }
    }
}
