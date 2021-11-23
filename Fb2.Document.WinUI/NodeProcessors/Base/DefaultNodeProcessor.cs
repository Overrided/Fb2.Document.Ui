using System;
using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Models.Base;
using Fb2.Document.WinUI.Entities;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors.Base
{
    public class DefaultNodeProcessor : NodeProcessorBase
    {
        public override List<TextElement> Process(IRenderingContext context)
        {
            var currentNode = context.Node;

            if (currentNode is Fb2Container containerNode)
            {
                return containerNode.Content
                    .Select(n => ElementSelector(n, context))
                    .OfType<List<TextElement>>() //    .Where(n => n != null && n.Any())
                    .SelectMany(l => l)
                    .ToList();
            }
            else if (currentNode is Fb2Element)// TODO : use .Content in new lib version, once .net6 comes to uwp (winUi 3.0)
                return new List<TextElement>(1) { new Run { Text = currentNode.ToString() } };
            else
                throw new Exception($"Unsupported node type. Expected Fb2Container or Fb2Element, got {currentNode.GetType()} instead.");
        }
    }
}
