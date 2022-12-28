using System.Collections.Generic;
using Fb2.Document.UWP.Entities;
using Fb2.Document.UWP.NodeProcessors.Base;
using Windows.UI.Xaml.Documents;

namespace Fb2.Document.UWP.NodeProcessors
{
    public class ParagraphProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var inlines = base.Process(context);

            //var testHyperlink = new Hyperlink();
            //testHyperlink.Inlines.Add(new Run { Text = "test inline HYPERLINK" });
            //inlines.Insert(0, testHyperlink);

            var paragraphs = context.Utils.Paragraphize(inlines);

            return paragraphs;
        }
    }
}
