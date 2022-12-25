﻿using System.Collections.Generic;
using System.Linq;
using Fb2.Document.Constants;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.NodeProcessors.Base;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors
{
    public class CustomInfoProcessor : DefaultNodeProcessor
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var processedInlines = base.Process(context);

            var currentNode = context.CurrentNode;

            if ((currentNode?.TryGetAttribute(AttributeNames.InfoType, true, out var infoTypeKvp) ?? false) &&
                !string.IsNullOrEmpty(infoTypeKvp?.Value))
            {
                var attributeRun = new Run { Text = infoTypeKvp.Value };
                processedInlines.Insert(0, attributeRun);
            }

            return context.Utils.Paragraphize(processedInlines);
        }
    }
}
