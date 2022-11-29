using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.Entities;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.UI.NodeProcessors.Base
{
    public class DefaultNodeProcessor : NodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var currentNode = context.CurrentNode;

            if (currentNode is Fb2Container containerNode)
                return containerNode.Content
                    .Select(n => ElementSelector(n, context))
                    .OfType<List<TextElement>>() //    .Where(n => n != null && n.Any())
                    .SelectMany(l => l)
                    .ToList();

            if (currentNode is Fb2Element elementNode)
                return new List<TextElement>(1) { new Run { Text = elementNode.Content } };

            throw new Exception($"Unsupported node type. Expected {nameof(Fb2Container)} or {nameof(Fb2Element)}, got {currentNode.GetType()} instead.");
        }
    }
}
