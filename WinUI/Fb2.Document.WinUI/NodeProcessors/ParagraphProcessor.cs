using System.Collections.Generic;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors;

public class ParagraphProcessor : DefaultNodeProcessor
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var inlines = base.Process(context);

        //var testHyperlink = new Hyperlink();
        //testHyperlink.Inlines.Add(new Run { Text = "test inline HYPERLINK" });
        //inlines.Insert(0, testHyperlink);

        var paragraphs = context.Utils.Paragraphize(inlines);

        return paragraphs;
    }
}
