﻿using System.Collections.Generic;
using Fb2.Document.WinUI.Entities;
using Fb2.Document.WinUI.Extensions;
using Microsoft.UI.Xaml.Documents;

namespace Fb2.Document.WinUI.NodeProcessors.Base
{
    public abstract class SpanProcessorBase<T> : DefaultNodeProcessor where T : Span, new()
    {
        public override List<TextElement> Process(RenderingContext context)
        {
            var inlines = base.Process(context);

            var result = new T();
            result.Inlines.AddRange(inlines);

            return new List<TextElement>(1) { result };
        }
    }

    public class BoldProcessor : SpanProcessorBase<Bold> { }
    public class ItalicProcessor : SpanProcessorBase<Italic> { }
    public class SpanProcessor : SpanProcessorBase<Span> { }
}
