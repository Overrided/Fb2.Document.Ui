using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using Fb2.Document.WPF.NodeProcessors.Base;
using Fb2.Document.WPF.Entities;
using System.Windows.Controls;

namespace Fb2.Document.WPF.NodeProcessors;

public class SubscriptProcessor : RewrapNodeProcessorBase
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var normalizedContent = base.Process(context);

        //var txtb = new FlowDocument
        //{
        //    FontSize = context.RenderingConfig.BaseFontSize
        //};
        //txtb.Blocks.AddRange(normalizedContent);

        //var txtbContainer = new FlowDocumentReader
        //{
        //    Margin = new Thickness(0, 1, 0, -10),
        //    Padding = new Thickness(0, 0, 0, 1)
        //};
        //txtbContainer.Document = txtb;

        //var inlineContainer = AddContainer(txtbContainer);

        //return new List<TextElement>(1) { inlineContainer };

        var span = new Span();
        span.Inlines.AddRange(normalizedContent);
        span.BaselineAlignment = BaselineAlignment.Subscript;

        return new List<TextElement>(1) { span };
    }
}