using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.Common
{
    public class Utils
    {
        private static readonly Lazy<Utils> instance = new(() => new Utils(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static Utils Instance => instance.Value;

        private Utils() { }

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
                if (element is Block paragElement)
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

        /// <summary>
        /// Returns the number of steps required to transform the source string
        /// into the target string.
        /// </summary> 
        public int GetEditingDistance(string source, string target)
        {
            // corner cases
            if (source == target ||
                string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target)) return 0;

            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            int sourceCharCount = source.Length;
            int targetCharCount = target.Length;

            int[,] distance = new int[sourceCharCount + 1, targetCharCount + 1];

            // Step 2
            for (int i = 0; i <= sourceCharCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetCharCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceCharCount; i++)
            {
                for (int j = 1; j <= targetCharCount; j++)
                {
                    // Step 3
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceCharCount, targetCharCount];
        }
    }
}
