using System;
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
        private const string TagSeparator = "|";

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

        protected static void SetTooltip(DependencyObject target, string tooltipText)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (string.IsNullOrWhiteSpace(tooltipText))
                throw new ArgumentNullException(nameof(tooltipText));

            ToolTip toolTip = new ToolTip { Content = tooltipText };
            ToolTipService.SetToolTip(target, toolTip);
        }

        protected static bool TagElement(DependencyObject element, string tag)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (string.IsNullOrEmpty(tag))
                throw new ArgumentNullException(nameof(tag));

            if (element is not FrameworkElement frameworkElement)
                return false;

            var existingTag = frameworkElement.Tag?.ToString();
            var newTag = string.IsNullOrEmpty(existingTag) ? tag : $"{existingTag}{TagSeparator}{tag}";
            frameworkElement.Tag = newTag;
            return true;
        }
    }
}
