using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace RichTextView.UWP.Common
{
    internal static class PaginationUtils
    {
        internal static List<TextElement> Paragraphize(IEnumerable<TextElement> elements)
        {
            Paragraph actualParagraph = null;

            var result = new List<TextElement>();

            foreach (var element in elements)
            {
                if (element is Block paragraphElement)
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
