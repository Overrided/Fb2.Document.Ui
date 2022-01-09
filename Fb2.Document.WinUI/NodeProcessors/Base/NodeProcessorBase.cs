using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors.Base
{
    public abstract class NodeProcessorBase
    {
        public abstract List<TextElement> Process(IRenderingContext context);

        public List<TextElement> ElementSelector(Fb2Node node, IRenderingContext context)
        {
            context.UpdateNode(node);

            var processor = context.ProcessorFactory.GetNodeProcessor(node);
            var result = processor.Process(context);

            if (result?.Any() ?? false)
                context.Styler.ApplyStyle(context, result);

            context.Backtrack();

            return result;
        }

        protected void SetTooltip(DependencyObject target, string tooltipText)
        {
            if (string.IsNullOrWhiteSpace(tooltipText))
                return;

            ToolTip toolTip = new ToolTip { Content = tooltipText };
            ToolTipService.SetToolTip(target, toolTip);
        }
    }
}
