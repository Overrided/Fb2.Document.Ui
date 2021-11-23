using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Common;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors.Base
{
    public abstract class NodeProcessorBase
    {
        private HashSet<string> PageStarterTypeNames = new HashSet<string>
        {
            ElementNames.BookBody,
            ElementNames.BookBodySection,
            ElementNames.Coverpage
        };

        public abstract List<TextElement> Process(IRenderingContext context);

        public List<TextElement> ElementSelector(Fb2Node node, IRenderingContext context)
        {
            context.UpdateNode(node);

            var processor = context.ProcessorFactory.GetNodeProcessor(node);
            var result = processor.Process(context);

            if (result?.Any() ?? false)
            {
                context.Styler.ApplyStyle(context, result);
                TagElements(context, result);
            }

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

        private void TagElements(IRenderingContext context, List<TextElement> elements)
        {
            var currentNode = context.Node;

            if (currentNode.TryGetAttribute(AttributeNames.Id, out var idAttr, true))
                context.DependencyPropertyManager.AddOrUpdateProperty(elements.First(), idAttr.Key, idAttr.Value);

            var currentNodeName = currentNode.Name;

            if (PageStarterTypeNames.Contains(currentNodeName))
                context.DependencyPropertyManager.AddOrUpdateProperty(elements.First(), Fb2UIConstants.ContainerTypeAttributeName, currentNodeName);
        }
    }
}
