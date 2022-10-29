using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.WinUi.Common
{
    public class Utils
    {
        public List<TextElement> Paragraphize(params TextElement[] elements)
        {
            if (elements == null || !elements.Any())
                throw new ArgumentNullException(nameof(elements));

            // TODO : use OfType<TextElement> ?
            return Paragraphize(elements.Where(e => e != null));
        }

        public List<TextElement> Paragraphize(IEnumerable<TextElement> elements)
        {
            Paragraph actualParagraph = null;
            var result = new List<TextElement>();

            foreach (var element in elements)
            {
                if (element is Paragraph paragElement)
                {
                    if (actualParagraph != null)
                    {
                        result.Add(actualParagraph);
                        actualParagraph = null;
                    }
                    result.Add(paragElement);
                }
                else if (element is Inline inlineElem)
                {
                    if (actualParagraph == null)
                        actualParagraph = new Paragraph();

                    actualParagraph.Inlines.Add(inlineElem);
                }
            }

            if (actualParagraph != null)
                result.Add(actualParagraph);

            return result;
        }
    }
}
